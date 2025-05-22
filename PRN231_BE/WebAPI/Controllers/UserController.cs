using DataAccess.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDTO userDto)
        {
            var response = await _userService.CreateUserAsync(userDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UserUpdateDTO userDto)
        {
            var response = await _userService.UpdateUserAsync(userDto);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetUsersPaged(int pageNumber = 1, int pageSize = 10)
        {
            var response = await _userService.GetAllUserAsync(pageNumber, pageSize);
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }



    }
}
