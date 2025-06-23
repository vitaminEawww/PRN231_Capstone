using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using Services.Services;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccess.Common;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipUsageController : ControllerBase
    {
        private readonly IMembershipUsage _membershipUsage;

        public MembershipUsageController(IMembershipUsage membershipUsage)
        {
            _membershipUsage = membershipUsage;
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
            // Lấy customerId từ JWT claims
            var customerId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (customerId == 0)
            {
                return BadRequest(new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorMessages = new List<string> { "CustomerId is missing in claims." }
                });
            }

            var response = await _membershipUsage.GetMyMembershipStatusAsync(customerId);

            if (!response.IsSuccess)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response.Result);
        }
    }
}
