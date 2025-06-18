using System;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.Plans;

public class QuitPlanCreateDTO
{
    [Required(ErrorMessage = "Tiêu đề kế hoạch là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Lý do cai thuốc là bắt buộc")]
    [StringLength(1000, ErrorMessage = "Lý do không được vượt quá 1000 ký tự")]
    public string Reasons { get; set; } = null!;

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày mục tiêu là bắt buộc")]
    public DateTime TargetDate { get; set; }

    [StringLength(2000, ErrorMessage = "Ghi chú không được vượt quá 2000 ký tự")]
    public string? Notes { get; set; }

    /// <summary>
    /// True nếu muốn hệ thống tự động tạo các giai đoạn
    /// </summary>
    public bool AutoGeneratePhases { get; set; } = true;

    /// <summary>
    /// Danh sách các giai đoạn tùy chỉnh (nếu AutoGeneratePhases = false)
    /// </summary>
    public List<QuitPlanPhaseCreateDTO>? CustomPhases { get; set; }
}
