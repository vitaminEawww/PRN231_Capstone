using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using Services.Services;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipUsageController : ControllerBase
    {
        private readonly IMembershipUsage _membershipUsage;

        public MembershipUsageController( IMembershipUsage membershipUsage )
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
    }
}
