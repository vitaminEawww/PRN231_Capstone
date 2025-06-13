using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Auth;

public class LoginRequestDTO
{
    [Required(ErrorMessage = "Tên người dùng hoặc email là bắt buộc")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = null!;
}
