using System;
using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Models.Auth;

public class RegisterCustomerRequestDTO
{
    [Required(ErrorMessage = "Username là bắt buộc")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username phải từ 3-100 ký tự")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string Phone { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = null!;

    // Customer Profile Info
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Giới tính là bắt buộc")]
    public Gender? Gender { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }

    public (bool IsValid, string ErrorMessage) Validate()
    {
        if (Gender != null)
        {
            return (Enum.IsDefined(typeof(Gender), Gender), "Giới tính không hợp lệ");
        }
        return (true, "");
    }
}
