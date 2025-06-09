using DataAccess.Common;
using DataAccess.Models.Memberships;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers;

[Route("api/membership")]
[ApiController]
public class MembershipController(IMembershipService membershipService) : ControllerBase
{
    private readonly IMembershipService _membershipService = membershipService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAllMemberships()
    {
        var response = await _membershipService.GetAllMembershipsAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetMembershipById(int id)
    {
        var response = await _membershipService.GetMembershipByIdAsync(id);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateMembership(MembershipCreateDTO membershipDto)
    {
        var response = await _membershipService.CreateMembershipAsync(membershipDto);
        return Ok(response);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateMembership(MembershipUpdateDTO membershipDto)
    {
        var response = await _membershipService.UpdateMembershipAsync(membershipDto);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteMembership(int id)
    {
        var response = await _membershipService.DeleteMembershipAsync(id);
        return Ok(response);
    }
}