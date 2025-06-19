using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Models.Consultation;
using DataAccess.Common;
using DataAccess.Enums;
using System.Security.Claims;
using System.Collections.Generic;
using System.Net;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateConsultation([FromBody] CreateConsultationDTO dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _consultationService.CreateConsultationAsync(int.Parse(userId), dto);
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetConsultations([FromQuery] ConsultationStatus? status = null)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _consultationService.GetConsultationsAsync(int.Parse(userId), role ?? "", status);
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetConsultationById(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _consultationService.GetConsultationByIdAsync(int.Parse(userId), role ?? "", id);
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

        [HttpPut("{id}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateConsultationStatus(int id, [FromBody] UpdateConsultationStatusDTO dto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _consultationService.UpdateConsultationStatusAsync(int.Parse(userId), role ?? "", id, dto);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteConsultation(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userId == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "User is not authenticated." }
                    });
                }

                var result = await _consultationService.DeleteConsultationAsync(int.Parse(userId), role ?? "", id);
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
    }
} 