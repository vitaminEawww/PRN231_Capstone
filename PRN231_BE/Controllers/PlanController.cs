using DataAccess.Models.Plans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlanController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPlans()
        {
            var response = await _planService.GetAllPlansAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(int id)
        {
            var response = await _planService.GetPlanByIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePlan([FromBody] PlanCreateDTO planDto)
        {
            var response = await _planService.CreatePlanAsync(planDto);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePlan([FromBody] PlanUpdateDTO planDto)
        {
            var response = await _planService.UpdatePlanAsync(planDto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var response = await _planService.DeletePlanAsync(id);
            return Ok(response);
        }

        [HttpPut("{planId}/phase/{phaseId}/progress")]
        public async Task<IActionResult> UpdatePlanProgress(int planId, int phaseId, [FromBody] double progress)
        {
            var response = await _planService.UpdatePlanProgressAsync(planId, phaseId, progress);
            return Ok(response);
        }
    }
}