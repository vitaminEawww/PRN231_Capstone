using DataAccess.Models.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using Services.Service;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private readonly IFeatureService _featureService;
        public FeatureController(IFeatureService featureService)
        {
            _featureService = featureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _featureService.GetAllAsync();
            return StatusCode((int)response.StatusCode, response.ErrorMessages);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _featureService.GetPagedAsync(pageNumber, pageSize);
            return StatusCode((int)response.StatusCode, response);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _featureService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FeatureCreateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Feature data is null.");
            }
            var response = await _featureService.CreateAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FeatureUpdateDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Feature data is null.");
            }
            var response = await _featureService.UpdateAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _featureService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
