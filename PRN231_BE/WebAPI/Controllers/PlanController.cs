using DataAccess.Common;
using DataAccess.Models.Plans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers;

[Route("api/plan")]
[ApiController]
public class PlanController(IPlanService planService) : ControllerBase
{
    private readonly IPlanService _planService = planService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAllPlans()
    {
        var response = await _planService.GetAllPlansAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetPlanById(int id)
    {
        var response = await _planService.GetPlanByIdAsync(id);
        return Ok(response);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreatePlan(PlanCreateDTO planDto)
    {
        var response = await _planService.CreatePlanAsync(planDto);
        return Ok(response);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdatePlan(PlanUpdateDTO planDto)
    {
        var response = await _planService.UpdatePlanAsync(planDto);
        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeletePlan(int id)
    {
        var response = await _planService.DeletePlanAsync(id);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{planId}/phase/{phaseId}/progress")]
    public async Task<ActionResult<ApiResponse>> UpdatePlanProgress(int planId, int phaseId, [FromBody] double progress)
    {
        var response = await _planService.UpdatePlanProgressAsync(planId, phaseId, progress);
        return Ok(response);
    }
}