using DataAccess.Common;
using DataAccess.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng
        /// </summary>
        [HttpPost("register-customer")]
        public async Task<ActionResult<ApiResponse>> RegisterCustomer([FromBody] RegisterCustomerRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var response = new ApiResponse
                    {
                        IsSuccess = false,
                        ErrorMessages = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    };
                    return BadRequest(response);
                }

                if (!request.Validate().IsValid) return BadRequest(new ApiResponse
                {
                    IsSuccess = request.Validate().IsValid,
                    ErrorMessages = new List<string> { request.Validate().ErrorMessage }
                });


                var result = await _authService.RegisterCustomerAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
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


        /// <summary>
        /// Đăng kí tài khoản coach
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register-coach")]
        public async Task<ActionResult<ApiResponse>> RegisterCoach([FromBody] RegisterCoachRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var response = new ApiResponse
                    {
                        IsSuccess = false,
                        ErrorMessages = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    };
                    return BadRequest(response);
                }

                if (!request.Validate().IsValid) return BadRequest(new ApiResponse
                {
                    IsSuccess = request.Validate().IsValid,
                    ErrorMessages = new List<string> { request.Validate().ErrorMessage }
                });

                var result = await _authService.RegisterCoachAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
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

        /// <summary>
        /// Đăng nhập
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var response = new ApiResponse
                    {
                        IsSuccess = false,
                        ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                    };
                    return BadRequest(response);
                }

                var result = await _authService.LoginAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
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

        /// <summary>
        /// Lấy thông tin profile người dùng hiện tại
        /// </summary>
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _authService.GetUserProfileAsync(userId);
                return Ok(result);
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

        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        #endregion
    }
}