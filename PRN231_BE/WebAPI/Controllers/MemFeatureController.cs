using DataAccess.Models.MemFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemFeatureController : ControllerBase
    {
        private readonly IMemFeatureService _service;

        public MemFeatureController(IMemFeatureService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync();
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("membership/{membershipId}")]
        public async Task<IActionResult> GetByMembership(int membershipId)
        {
            var response = await _service.GetByMembershipIdAsync(membershipId);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemFeatureCreateDTO dto)
        {
            var response = await _service.CreateAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
