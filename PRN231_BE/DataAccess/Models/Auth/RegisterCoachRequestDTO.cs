using System;
using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Models.Auth;

public class RegisterCoachRequestDTO
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

    // Common Profile Info
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(200, ErrorMessage = "Họ tên không được vượt quá 200 ký tự")]
    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }

    // Customer specific fields
    public Gender? Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    // Coach specific fields
    [StringLength(500, ErrorMessage = "Chuyên môn không được vượt quá 500 ký tự")]
    public string? Specialization { get; set; }

    [StringLength(1000, ErrorMessage = "Bằng cấp không được vượt quá 1000 ký tự")]
    public string? Qualifications { get; set; }

    [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0-50")]
    public int? ExperienceYears { get; set; }

    [Range(0, 10000000, ErrorMessage = "Giá tiền/giờ phải từ 0-10,000,000 VNĐ")]
    public decimal? HourlyRate { get; set; }

    public (bool IsValid, string ErrorMessage) Validate()
    {
        if (Gender != null) return (Enum.IsDefined(typeof(Gender), Gender), "Giới tính không hợp lệ");

        return (true, string.Empty);
    }
}
