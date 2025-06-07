using DataAccess.Models.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => StatusCode((int)(await _commentService.GetAllAsync()).StatusCode, await _commentService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => StatusCode((int)(await _commentService.GetByIdAsync(id)).StatusCode, await _commentService.GetByIdAsync(id));

        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetByPostId(int postId) => StatusCode((int)(await _commentService.GetByPostIdAsync(postId)).StatusCode, await _commentService.GetByPostIdAsync(postId));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentCreateDTO dto) => StatusCode((int)(await _commentService.CreateAsync(dto)).StatusCode, await _commentService.CreateAsync(dto));

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CommentUpdateDTO dto) => StatusCode((int)(await _commentService.UpdateAsync(dto)).StatusCode, await _commentService.UpdateAsync(dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => StatusCode((int)(await _commentService.DeleteAsync(id)).StatusCode, await _commentService.DeleteAsync(id));
    }
}
