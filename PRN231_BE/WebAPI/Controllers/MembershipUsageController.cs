using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using Services.Services;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Common;
using Repositories.IRepositories;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipUsageController : ControllerBase
    {
        private readonly IMembershipUsage _membershipUsage;
        private readonly IUnitOfWork _unitOfWork;

        public MembershipUsageController(IMembershipUsage membershipUsage, IUnitOfWork unitOfWork)
        {
            _membershipUsage = membershipUsage;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("usage/{userId}")]
        public async Task<IActionResult> GetMemberShipUsage(int userId)
        {
            var response = await _membershipUsage.GetMemberShipUsageAsync(userId);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response.Result); 
        }

        // API kiểm tra trạng thái membership hiện tại của user
        [Authorize]
        [HttpGet("my-membership")]
        public async Task<IActionResult> GetMyMembershipStatus()
        {
            // Lấy userId từ JWT claims
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "UserId is missing in claims." }
                });
            }

            // Lấy CustomerId từ UserId
            var customer = await _unitOfWork.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer == null)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "Customer not found for this user." }
                });
            }

            var response = await _membershipUsage.GetMyMembershipStatusAsync(customer.Id);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response.Result);
        }
    }
}
