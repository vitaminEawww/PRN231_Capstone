using DataAccess.Models.Posts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _postService.GetAllAsync();
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _postService.GetByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostCreateDTO dto)
        {
            var response = await _postService.CreateAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PostUpdateDTO dto)
        {
            var response = await _postService.UpdateAsync(dto);
            return StatusCode((int)response.StatusCode, response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _postService.DeleteAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
