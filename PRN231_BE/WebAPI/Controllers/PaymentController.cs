using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Services.IServices;
using DataAccess.Models.Payment;
using DataAccess.Enums;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.MembershipPackage;
using Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Data;
using System.Reflection;
using System.Web;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public PaymentController(IVnPayService vnPayService, IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _vnPayService = vnPayService;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        // API tạo URL thanh toán VNPAY
        [Authorize]
        [HttpPost("purchase-package")]
        public async Task<IActionResult> PurchasePackageAsync([FromBody] PurchasePackageDTO purchaseDto)
        {
            // Lấy customerId từ JWT claims
            var customerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (customerId == 0)
            {
                return BadRequest("CustomerId is missing in claims.");
            }

            try
            {
                // Kiểm tra sự tồn tại của gói dịch vụ
                var package = await _unitOfWork.MembershipPackages.GetByIdAsync(purchaseDto.MembershipPackageId);
                if (package == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessages = new List<string> { "Package not found." }
                    });
                }

                // Tạo thông tin thanh toán
                var paymentInfo = new PaymentInformationModel
                {
                    OrderId = new Random().Next(100000, 999999),  // Tạo OrderId ngẫu nhiên
                    Amount = (int)package.Price,  // Lấy giá tiền từ gói
                    OrderDescription = package.Name,
                    OrderType = "MembershipPackage",  // Loại đơn hàng là gói dịch vụ
                };

                // Tạo URL thanh toán VNPAY và truyền this.HttpContext vào
                var paymentUrl = _vnPayService.CreatePaymentUrl(paymentInfo, this.HttpContext);

                // Lưu thông tin thanh toán vào database
                var payment = new Payment
                {
                    CustomerId = customerId, // Lấy từ JWT
                    MembershipPackageId = purchaseDto.MembershipPackageId,
                    Amount = package.Price,  // Lấy giá tiền từ gói
                    Status = PaymentStatus.Pending,  // Trạng thái ban đầu là "Pending"
                    PaymentDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    TransactionId = paymentInfo.OrderId.ToString(), // Lưu OrderId vào TransactionId
                    Method = PaymentMethod.Cash, // Set cứng nếu chỉ có 1 phương thức, hiện tại là Cash
                    Type = PaymentType.Membership,
                    Description = "Payment for " + package.Name
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveAsync();

                // Trả về URL thanh toán cho người dùng
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.OK,
                    Result = paymentUrl  // Trả về URL thanh toán VNPAY
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayCallback([FromQuery] VNPayResponse model)
        {
            try
            {
                // Lấy thông tin trả về từ VNPAY
                var response = _vnPayService.PaymentExecute(Request.Query);

                // Kiểm tra kết quả thanh toán
                if (response.Success && response.VnPayResponseCode == "00")
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            // Tìm payment theo TransactionId
                            var payment = await _context.Payments
                                .FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);

                            if (payment != null)
                            {
                                // Nếu thanh toán đã hoàn thành rồi, tránh xử lý lại
                                if (payment.Status == PaymentStatus.Completed)
                                {
                                    return Ok(new ApiResponse
                                    {
                                        IsSuccess = true,
                                        StatusCode = HttpStatusCode.OK,
                                        Result = "Payment already processed."
                                    });
                                }

                                // Cập nhật trạng thái thanh toán
                                payment.Status = PaymentStatus.Completed;
                                payment.PaymentDate = DateTime.UtcNow;

                                // Lưu thông tin payment đã cập nhật
                                await _context.SaveChangesAsync();

                                // Cập nhật thông tin người dùng (Customer)
                                var customer = await _context.Customers
                                    .FirstOrDefaultAsync(c => c.Id == payment.CustomerId);

                                // Kiểm tra người dùng đã có usage active chưa
                                var existingUsage = await _context.MemberShipUsages
                                    .FirstOrDefaultAsync(u => u.CustomerId == payment.CustomerId && u.Status == PackageStatus.Active);

                                if (existingUsage == null)
                                {
                                    // Tạo mới MemberShipUsage
                                    var package = await _context.MembershipPackages
                                        .FirstOrDefaultAsync(p => p.Id == payment.MembershipPackageId);

                                    if (package != null)
                                    {
                                        var duration = package.DurationInDays; // Sử dụng gói dịch vụ này bao lâu

                                        var userUsage = new MemberShipUsage
                                        {
                                            CustomerId = payment.CustomerId,
                                            MembershipPackageId = payment.MembershipPackageId.Value,
                                            StartDate = DateTime.UtcNow,
                                            EndDate = DateTime.UtcNow.AddDays(duration),
                                            Status = PackageStatus.Active
                                        };

                                        // Thêm MemberShipUsage mới
                                        await _context.MemberShipUsages.AddAsync(userUsage);
                                    }
                                }
                               
                                // Commit transaction
                                await transaction.CommitAsync();

                                return Ok(new ApiResponse
                                {
                                    IsSuccess = true,
                                    StatusCode = HttpStatusCode.OK,
                                    Result = "Payment successfully processed"
                                });
                            }
                            else
                            {
                                return BadRequest("Payment not found.");
                            }
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return StatusCode(500, new ApiResponse
                            {
                                IsSuccess = false,
                                StatusCode = HttpStatusCode.InternalServerError,
                                ErrorMessages = new List<string> { "Internal server error" }
                            });
                        }
                    }
                }
                else
                {
                    return BadRequest(new ApiResponse
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.BadRequest,
                        ErrorMessages = new List<string> { "Payment failed" }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorMessages = new List<string> { "Internal Server Error" }
                });
            }
        }


        [HttpGet("vn-pay-return")]
        public async Task<IActionResult> VnPayReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.Success && response.VnPayResponseCode == "00")
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);

                        if (payment == null)
                            return Content("Không tìm thấy giao dịch thanh toán!");

                        if (payment.Status != PaymentStatus.Completed)
                        {
                            payment.Status = PaymentStatus.Completed;
                            payment.PaymentDate = DateTime.UtcNow;
                            await _context.SaveChangesAsync();

                            var existingUsage = await _context.MemberShipUsages
                                .FirstOrDefaultAsync(u => u.CustomerId == payment.CustomerId && u.Status == PackageStatus.Active);

                            if (existingUsage == null)
                            {
                                var package = await _context.MembershipPackages
                                    .FirstOrDefaultAsync(p => p.Id == payment.MembershipPackageId);
                                if (package != null)
                                {
                                    var duration = package.DurationInDays;
                                    var userUsage = new MemberShipUsage
                                    {
                                        CustomerId = payment.CustomerId,
                                        MembershipPackageId = payment.MembershipPackageId.Value,
                                        StartDate = DateTime.UtcNow,
                                        EndDate = DateTime.UtcNow.AddDays(duration),
                                        Status = PackageStatus.Active
                                    };
                                    await _context.MemberShipUsages.AddAsync(userUsage);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }

                        await transaction.CommitAsync();
                        return Content("Thanh toán thành công! Cảm ơn bạn đã sử dụng dịch vụ.");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return Content($"Có lỗi xảy ra khi xử lý thanh toán: {ex.Message}");
                    }
                }
            }
            else
            {
                return Content("Thanh toán thất bại hoặc bị hủy!");
            }
        }

        public class VNPayResponse
        {
            public string vnp_TmnCode { get; set; } = string.Empty;
            public string vnp_BankCode { get; set; } = string.Empty;
            public string? vnp_BankTranNo { get; set; } = string.Empty;
            public string vnp_CardType { get; set; } = string.Empty;
            public string vnp_OrderInfo { get; set; } = string.Empty;
            public string vnp_TransactionNo { get; set; } = string.Empty;
            public string vnp_TransactionStatus { get; set; } = string.Empty;
            public string vnp_TxnRef { get; set; } = string.Empty;
            public string? vnp_SecureHashType { get; set; } = string.Empty;
            public string vnp_SecureHash { get; set; } = string.Empty;
            public int? vnp_Amount { get; set; }
            public string? vnp_ResponseCode { get; set; }
            public string vnp_PayDate { get; set; } = string.Empty;

            public string ToUrlParameters()
            {
                var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var queryString = new List<string>();

                foreach (var property in properties)
                {
                    var value = property.GetValue(this);
                    if (value != null)
                    {
                        queryString.Add($"{property.Name}={HttpUtility.UrlEncode(value.ToString())}");
                    }
                }

                return string.Join("&", queryString);
            }
        }
    }
}
