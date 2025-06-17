using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Plans;

public class QuitPlanPhaseCreateDTO
{
    [Required(ErrorMessage = "Số thứ tự giai đoạn là bắt buộc")]
    public int PhaseNumber { get; set; }

    [Required(ErrorMessage = "Tiêu đề giai đoạn là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Mô tả giai đoạn là bắt buộc")]
    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Ngày bắt đầu giai đoạn là bắt buộc")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc giai đoạn là bắt buộc")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Mục tiêu số điếu thuốc/ngày là bắt buộc")]
    [Range(0, int.MaxValue, ErrorMessage = "Số điếu thuốc phải >= 0")]
    public int TargetCigarettesPerDay { get; set; }
}
