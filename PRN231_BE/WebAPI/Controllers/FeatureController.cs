using DataAccess.Common;
using DataAccess.Models.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers;

[Route("api/feature")]
[ApiController]
public class FeatureController(IFeatureService featureService) : ControllerBase
{
    private readonly IFeatureService _featureService = featureService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAllFeatures()
    {
        var response = await _featureService.GetAllFeaturesAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetFeatureById(int id)
    {
        var response = await _featureService.GetFeatureByIdAsync(id);
        return Ok(response);
    }

    [HttpGet("membership/{membershipId}")]
    public async Task<ActionResult<ApiResponse>> GetFeaturesByMembershipId(int membershipId)
    {
        var response = await _featureService.GetFeaturesByMembershipIdAsync(membershipId);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateFeature(FeatureCreateDTO featureDto)
    {
        var response = await _featureService.CreateFeatureAsync(featureDto);
        return Ok(response);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateFeature(FeatureUpdateDTO featureDto)
    {
        var response = await _featureService.UpdateFeatureAsync(featureDto);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteFeature(int id)
    {
        var response = await _featureService.DeleteFeatureAsync(id);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("membership")]
    public async Task<ActionResult<ApiResponse>> AddFeatureToMembership(MemFeatureCreateDTO memFeatureDto)
    {
        var response = await _featureService.AddFeatureToMembershipAsync(memFeatureDto);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("membership/{membershipId}/{featureId}")]
    public async Task<ActionResult<ApiResponse>> RemoveFeatureFromMembership(int membershipId, int featureId)
    {
        var response = await _featureService.RemoveFeatureFromMembershipAsync(membershipId, featureId);
        return Ok(response);
    }
}