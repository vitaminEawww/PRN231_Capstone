using System;
using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Models.Plans;

public class QuitPlanUpdateDTO
{
    [Required(ErrorMessage = "ID kế hoạch là bắt buộc")]
    public int Id { get; set; }

    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string? Title { get; set; }

    [StringLength(1000, ErrorMessage = "Lý do không được vượt quá 1000 ký tự")]
    public string? Reasons { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? TargetDate { get; set; }

    public PlanStatus? Status { get; set; }

    [StringLength(2000, ErrorMessage = "Ghi chú không được vượt quá 2000 ký tự")]
    public string? Notes { get; set; }
}
