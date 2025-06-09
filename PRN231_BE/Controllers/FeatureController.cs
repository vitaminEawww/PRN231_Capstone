using DataAccess.Models.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureService _featureService;

        public FeatureController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFeatures()
        {
            var response = await _featureService.GetAllFeaturesAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeatureById(int id)
        {
            var response = await _featureService.GetFeatureByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("membership/{membershipId}")]
        public async Task<IActionResult> GetFeaturesByMembershipId(int membershipId)
        {
            var response = await _featureService.GetFeaturesByMembershipIdAsync(membershipId);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateFeature([FromBody] FeatureCreateDTO featureDto)
        {
            var response = await _featureService.CreateFeatureAsync(featureDto);
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateFeature([FromBody] FeatureUpdateDTO featureDto)
        {
            var response = await _featureService.UpdateFeatureAsync(featureDto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFeature(int id)
        {
            var response = await _featureService.DeleteFeatureAsync(id);
            return Ok(response);
        }

        [HttpPost("membership")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddFeatureToMembership([FromBody] MemFeatureCreateDTO memFeatureDto)
        {
            var response = await _featureService.AddFeatureToMembershipAsync(memFeatureDto);
            return Ok(response);
        }

        [HttpDelete("membership/{membershipId}/feature/{featureId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveFeatureFromMembership(int membershipId, int featureId)
        {
            var response = await _featureService.RemoveFeatureFromMembershipAsync(membershipId, featureId);
            return Ok(response);
        }
    }
}