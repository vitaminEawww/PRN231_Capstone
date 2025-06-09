using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembershipController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMemberships()
        {
            var response = await _membershipService.GetAllMembershipsAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMembershipById(int id)
        {
            var response = await _membershipService.GetMembershipByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMembership([FromBody] MembershipCreateDTO membershipDto)
        {
            var response = await _membershipService.CreateMembershipAsync(membershipDto);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateMembership([FromBody] MembershipUpdateDTO membershipDto)
        {
            var response = await _membershipService.UpdateMembershipAsync(membershipDto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteMembership(int id)
        {
            var response = await _membershipService.DeleteMembershipAsync(id);
            return Ok(response);
        }
    }
}