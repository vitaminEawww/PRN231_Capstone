using DataAccess.Common;
using DataAccess.Models.Plans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers;

[Route("api/phase")]
[ApiController]
public class PhaseController(IPhaseService phaseService) : ControllerBase
{
    private readonly IPhaseService _phaseService = phaseService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAllPhases()
    {
        var response = await _phaseService.GetAllPhasesAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetPhaseById(int id)
    {
        var response = await _phaseService.GetPhaseByIdAsync(id);
        return Ok(response);
    }

    [HttpGet("plan/{planId}")]
    public async Task<ActionResult<ApiResponse>> GetPhasesByPlanId(int planId)
    {
        var response = await _phaseService.GetPhasesByPlanIdAsync(planId);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreatePhase(PhaseCreateDTO phaseDto)
    {
        var response = await _phaseService.CreatePhaseAsync(phaseDto);
        return Ok(response);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdatePhase(PhaseUpdateDTO phaseDto)
    {
        var response = await _phaseService.UpdatePhaseAsync(phaseDto);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeletePhase(int id)
    {
        var response = await _phaseService.DeletePhaseAsync(id);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id}/progress")]
    public async Task<ActionResult<ApiResponse>> UpdatePhaseProgress(int id, [FromBody] double progress)
    {
        var response = await _phaseService.UpdatePhaseProgressAsync(id, progress);
        return Ok(response);
    }
}