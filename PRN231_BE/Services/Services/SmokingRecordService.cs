using System.Net;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Models.SmokingRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services
{
    public class SmokingRecordService : ISmokingRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SmokingRecordService> _logger;

        public SmokingRecordService(
            IUnitOfWork unitOfWork,
            ILogger<SmokingRecordService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ApiResponse> CreateSmokingRecordAsync(SmokingRecordCreateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                // Validate customer exists
                var customer = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
                if (customer == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Không tìm thấy khách hàng");
                    return response;
                }

                // Check if customer already has a current smoking record
                var existingRecord = (await _unitOfWork.SmokingRecords.GetAllAsync())
                    .Where(sr => sr.CustomerId == dto.CustomerId && sr.QuitSmokingStartDate == null)
                    .OrderByDescending(sr => sr.CreatedAt)
                    .FirstOrDefault();

                if (existingRecord != null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.ErrorMessages.Add("Khách hàng đã có hồ sơ hút thuốc hiện tại. Vui lòng cập nhật thay vì tạo mới.");
                    return response;
                }

                var smokingRecord = new SmokingRecord
                {
                    CustomerId = dto.CustomerId,
                    CigarettesPerDay = dto.CigarettesPerDay,
                    CostPerPack = dto.CostPerPack,
                    CigarettesPerPack = dto.CigarettesPerPack,
                    Frequency = dto.Frequency,
                    Brands = dto.Brands,
                    Triggers = dto.Triggers,
                    SmokingStartDate = dto.SmokingStartDate,
                    SmokingYears = dto.SmokingYears,
                    RecordDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.SmokingRecords.AddAsync(smokingRecord);
                await _unitOfWork.SaveAsync();

                var responseDto = new SmokingRecordResponseDTO
                {
                    Id = smokingRecord.Id,
                    CustomerId = smokingRecord.CustomerId,
                    CigarettesPerDay = smokingRecord.CigarettesPerDay,
                    CostPerPack = smokingRecord.CostPerPack,
                    CigarettesPerPack = smokingRecord.CigarettesPerPack,
                    Frequency = smokingRecord.Frequency,
                    Brands = smokingRecord.Brands,
                    Triggers = smokingRecord.Triggers,
                    SmokingStartDate = smokingRecord.SmokingStartDate,
                    QuitSmokingStartDate = smokingRecord.QuitSmokingStartDate,
                    SmokingYears = smokingRecord.SmokingYears,
                    RecordDate = smokingRecord.RecordDate,
                    CreatedAt = smokingRecord.CreatedAt,
                    UpdatedAt = smokingRecord.UpdatedAt
                };

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                response.Result = responseDto;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating smoking record for customer {CustomerId}", dto.CustomerId);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra khi tạo hồ sơ hút thuốc");
                return response;
            }
        }

        public async Task<ApiResponse> GetCurrentSmokingRecordAsync(int customerId)
        {
            var response = new ApiResponse();
            try
            {
                var smokingRecord = (await _unitOfWork.SmokingRecords.GetAllAsync())
                    .Where(sr => sr.CustomerId == customerId && sr.QuitSmokingStartDate == null)
                    .OrderByDescending(sr => sr.CreatedAt)
                    .FirstOrDefault();

                if (smokingRecord == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Không tìm thấy hồ sơ hút thuốc hiện tại");
                    return response;
                }

                var responseDto = new SmokingRecordResponseDTO
                {
                    Id = smokingRecord.Id,
                    CustomerId = smokingRecord.CustomerId,
                    CigarettesPerDay = smokingRecord.CigarettesPerDay,
                    CostPerPack = smokingRecord.CostPerPack,
                    CigarettesPerPack = smokingRecord.CigarettesPerPack,
                    Frequency = smokingRecord.Frequency,
                    Brands = smokingRecord.Brands,
                    Triggers = smokingRecord.Triggers,
                    SmokingStartDate = smokingRecord.SmokingStartDate,
                    QuitSmokingStartDate = smokingRecord.QuitSmokingStartDate,
                    SmokingYears = smokingRecord.SmokingYears,
                    RecordDate = smokingRecord.RecordDate,
                    CreatedAt = smokingRecord.CreatedAt,
                    UpdatedAt = smokingRecord.UpdatedAt
                };

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = responseDto;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current smoking record for customer {CustomerId}", customerId);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra khi lấy thông tin hút thuốc");
                return response;
            }
        }

        public async Task<ApiResponse> GetSmokingHistoryAsync(int customerId, int pageNumber = 1, int pageSize = 10)
        {
            var response = new ApiResponse();
            try
            {
                var smokingRecords = (await _unitOfWork.SmokingRecords.GetAllAsync())
                    .Where(sr => sr.CustomerId == customerId)
                    .OrderByDescending(sr => sr.CreatedAt)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(sr => new SmokingRecordResponseDTO
                    {
                        Id = sr.Id,
                        CustomerId = sr.CustomerId,
                        CigarettesPerDay = sr.CigarettesPerDay,
                        CostPerPack = sr.CostPerPack,
                        CigarettesPerPack = sr.CigarettesPerPack,
                        Frequency = sr.Frequency,
                        Brands = sr.Brands,
                        Triggers = sr.Triggers,
                        SmokingStartDate = sr.SmokingStartDate,
                        QuitSmokingStartDate = sr.QuitSmokingStartDate,
                        SmokingYears = sr.SmokingYears,
                        RecordDate = sr.RecordDate,
                        CreatedAt = sr.CreatedAt,
                        UpdatedAt = sr.UpdatedAt
                    })
                    .ToList();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = new
                {
                    Data = smokingRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = smokingRecords.Count
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting smoking history for customer {CustomerId}", customerId);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra khi lấy lịch sử hút thuốc");
                return response;
            }
        }

        public async Task<ApiResponse> UpdateSmokingRecordAsync(int id, SmokingRecordUpdateDTO dto)
        {
            var response = new ApiResponse();
            try
            {
                var smokingRecord = await _unitOfWork.SmokingRecords.GetByIdAsync(id);
                if (smokingRecord == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Không tìm thấy bản ghi hút thuốc");
                    return response;
                }

                // Map only non-null values
                if (dto.CigarettesPerDay.HasValue)
                    smokingRecord.CigarettesPerDay = dto.CigarettesPerDay.Value;

                if (dto.CostPerPack.HasValue)
                    smokingRecord.CostPerPack = dto.CostPerPack.Value;

                if (dto.CigarettesPerPack.HasValue)
                    smokingRecord.CigarettesPerPack = dto.CigarettesPerPack.Value;

                if (dto.Frequency.HasValue)
                    smokingRecord.Frequency = dto.Frequency.Value;

                if (dto.Brands != null)
                    smokingRecord.Brands = dto.Brands;

                if (dto.Triggers != null)
                    smokingRecord.Triggers = dto.Triggers;

                if (dto.SmokingStartDate.HasValue)
                    smokingRecord.SmokingStartDate = dto.SmokingStartDate;

                if (dto.QuitSmokingStartDate.HasValue)
                    smokingRecord.QuitSmokingStartDate = dto.QuitSmokingStartDate;

                if (dto.SmokingYears.HasValue)
                    smokingRecord.SmokingYears = dto.SmokingYears;

                smokingRecord.UpdatedAt = DateTime.Now;

                _unitOfWork.SmokingRecords.Update(smokingRecord);
                await _unitOfWork.SaveAsync();

                var responseDto = new SmokingRecordResponseDTO
                {
                    Id = smokingRecord.Id,
                    CustomerId = smokingRecord.CustomerId,
                    CigarettesPerDay = smokingRecord.CigarettesPerDay,
                    CostPerPack = smokingRecord.CostPerPack,
                    CigarettesPerPack = smokingRecord.CigarettesPerPack,
                    Frequency = smokingRecord.Frequency,
                    Brands = smokingRecord.Brands,
                    Triggers = smokingRecord.Triggers,
                    SmokingStartDate = smokingRecord.SmokingStartDate,
                    QuitSmokingStartDate = smokingRecord.QuitSmokingStartDate,
                    SmokingYears = smokingRecord.SmokingYears,
                    RecordDate = smokingRecord.RecordDate,
                    CreatedAt = smokingRecord.CreatedAt,
                    UpdatedAt = smokingRecord.UpdatedAt
                };

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = responseDto;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating smoking record {Id}", id);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra khi cập nhật hồ sơ hút thuốc");
                return response;
            }
        }

        public async Task<ApiResponse> DeleteSmokingRecordAsync(int id)
        {
            var response = new ApiResponse();
            try
            {
                var smokingRecord = await _unitOfWork.SmokingRecords.GetByIdAsync(id);
                if (smokingRecord == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Không tìm thấy bản ghi hút thuốc");
                    return response;
                }

                _unitOfWork.SmokingRecords.Delete(smokingRecord);
                await _unitOfWork.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = "Xóa bản ghi thành công";
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting smoking record {Id}", id);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra khi xóa bản ghi hút thuốc");
                return response;
            }
        }

        public async Task<ApiResponse> GetSmokingRecordByIdAsync(int id)
        {
            var response = new ApiResponse();
            try
            {
                var smokingRecord = await _unitOfWork.SmokingRecords.GetByIdAsync(id);
                if (smokingRecord == null)
                {
                    response.IsSuccess = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Không tìm thấy bản ghi hút thuốc");
                    return response;
                }

                var responseDto = new SmokingRecordResponseDTO
                {
                    Id = smokingRecord.Id,
                    CustomerId = smokingRecord.CustomerId,
                    CigarettesPerDay = smokingRecord.CigarettesPerDay,
                    CostPerPack = smokingRecord.CostPerPack,
                    CigarettesPerPack = smokingRecord.CigarettesPerPack,
                    Frequency = smokingRecord.Frequency,
                    Brands = smokingRecord.Brands,
                    Triggers = smokingRecord.Triggers,
                    SmokingStartDate = smokingRecord.SmokingStartDate,
                    QuitSmokingStartDate = smokingRecord.QuitSmokingStartDate,
                    SmokingYears = smokingRecord.SmokingYears,
                    RecordDate = smokingRecord.RecordDate,
                    CreatedAt = smokingRecord.CreatedAt,
                    UpdatedAt = smokingRecord.UpdatedAt
                };

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                response.Result = responseDto;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting smoking record {Id}", id);
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorMessages.Add("Có lỗi xảy ra khi lấy thông tin hút thuốc");
                return response;
            }
        }
    }
}