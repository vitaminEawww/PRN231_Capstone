using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models.Coach;
using DataAccess.Models.Consultation;
using DataAccess.Common;
using DataAccess.Enums;
using System.Security.Claims;
using System.Collections.Generic;
using System.Net;
using Repositories.IRepositories;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoachController : ControllerBase
    {
        private readonly ICoachService _coachService;
        private readonly IUnitOfWork _unitOfWork;

        public CoachController(ICoachService coachService, IUnitOfWork unitOfWork)
        {
            _coachService = coachService;
            _unitOfWork = unitOfWork;
        }

        // GET /api/coaches - Lấy danh sách tất cả coach (cho customer chọn)
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllCoaches()
        {
            try
            {
                var result = await _coachService.GetAllCoachesAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // GET /api/coaches/{id} - Lấy thông tin chi tiết 1 coach
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetCoachById(int id)
        {
            try
            {
                var result = await _coachService.GetCoachByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Coach không tồn tại" }
                    });
                }

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // GET /api/coaches/profile - Lấy thông tin profile của coach hiện tại
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse>> GetCoachProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _coachService.GetCoachProfileAsync(userId.Value);
                if (result == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Coach không tồn tại" }
                    });
                }

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // PUT /api/coaches/profile - Cập nhật thông tin profile của coach
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse>> UpdateCoachProfile([FromBody] UpdateCoachProfileDTO dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _coachService.UpdateCoachProfileAsync(userId.Value, dto);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // GET /api/coaches/consultations - Lấy danh sách buổi tư vấn của coach
        [HttpGet("consultations")]
        public async Task<ActionResult<ApiResponse>> GetCoachConsultations([FromQuery] ConsultationStatus? status = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _coachService.GetCoachConsultationsAsync(userId.Value, status);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // GET /api/coaches/consultations/{id} - Lấy chi tiết 1 buổi tư vấn của coach
        [HttpGet("consultations/{id}")]
        public async Task<ActionResult<ApiResponse>> GetCoachConsultationById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _coachService.GetCoachConsultationByIdAsync(userId.Value, id);
                if (result == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Buổi tư vấn không tồn tại" }
                    });
                }

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // PUT /api/coaches/consultations/{id}/status - Coach cập nhật trạng thái buổi tư vấn
        [HttpPut("consultations/{id}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateConsultationStatus(int id, [FromBody] UpdateConsultationStatusDTO dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _coachService.UpdateConsultationStatusAsync(userId.Value, id, dto);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    ErrorMessages = new List<string> { ex.Message }
                });
            }
        }

        // Helper: Lấy UserId từ token
        private int? GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null) return null;
            if (!int.TryParse(userIdStr, out int userId)) return null;
            return userId;
        }
    }
} 