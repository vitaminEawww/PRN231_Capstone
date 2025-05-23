using DataAccess.Models.Plans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PhaseController : ControllerBase
    {
        private readonly IPhaseService _phaseService;

        public PhaseController(IPhaseService phaseService)
        {
            _phaseService = phaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPhases()
        {
            var response = await _phaseService.GetAllPhasesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhaseById(int id)
        {
            var response = await _phaseService.GetPhaseByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("plan/{planId}")]
        public async Task<IActionResult> GetPhasesByPlanId(int planId)
        {
            var response = await _phaseService.GetPhasesByPlanIdAsync(planId);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePhase([FromBody] PhaseCreateDTO phaseDto)
        {
            var response = await _phaseService.CreatePhaseAsync(phaseDto);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePhase([FromBody] PhaseUpdateDTO phaseDto)
        {
            var response = await _phaseService.UpdatePhaseAsync(phaseDto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePhase(int id)
        {
            var response = await _phaseService.DeletePhaseAsync(id);
            return Ok(response);
        }

        [HttpPut("{id}/progress")]
        public async Task<IActionResult> UpdatePhaseProgress(int id, [FromBody] double progress)
        {
            var response = await _phaseService.UpdatePhaseProgressAsync(id, progress);
            return Ok(response);
        }
    }
}