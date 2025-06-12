using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

public class QuitPlan
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Title { get; set; } = null!;
    public string Reasons { get; set; } = null!; // Lý do cai thuốc
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; } // Ngày dự kiến cai được
    public PlanStatus Status { get; set; }
    public bool IsSystemGenerated { get; set; } = false; // Hệ thống tạo hay user tạo
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual ICollection<QuitPlanPhase> Phases { get; set; } = new List<QuitPlanPhase>();
}
