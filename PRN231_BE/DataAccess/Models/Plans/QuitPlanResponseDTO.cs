using System;
using DataAccess.Enums;

namespace DataAccess.Models.Plans;

public class QuitPlanResponseDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Reasons { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public PlanStatus Status { get; set; }
    public string StatusText { get; set; } = null!;
    public bool IsSystemGenerated { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public List<QuitPlanPhaseResponseDTO> Phases { get; set; } = new();

    // Thống kê tiến độ
    public double OverallProgress { get; set; }
    public int TotalPhases { get; set; }
    public int CompletedPhases { get; set; }
    public int DaysRemaining { get; set; }
    public int TotalDays { get; set; }
}
