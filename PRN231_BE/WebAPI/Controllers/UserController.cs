using DataAccess.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using DataAccess.Common;

namespace WebAPI.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register(UserCreateDTO userDto)
    {
        var response = await _userService.CreateUserAsync(userDto);
        return Ok(response);
    }

    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse>> UpdateUser(UserUpdateDTO userDto)
    {
        var response = await _userService.UpdateUserAsync(userDto);
        return Ok(response);
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<ApiResponse>> GetUsersPaged(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _userService.GetAllUserAsync(pageNumber, pageSize);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetUserById(int id)
    {
        var response = await _userService.GetUserByIdAsync(id);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
    {
        var response = await _userService.DeleteUserAsync(id);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginDTO loginDto)
    {
        var response = await _userService.LoginAsync(loginDto.Email, loginDto.Password);
        return Ok(response);
    }
}
