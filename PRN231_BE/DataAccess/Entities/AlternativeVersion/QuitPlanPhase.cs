using System;

namespace DataAccess.Entities.AlternativeVersion;

public class QuitPlanPhase
{
    public int Id { get; set; }
    public int QuitPlanId { get; set; }
    public int PhaseNumber { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TargetCigarettesPerDay { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    public virtual QuitPlan? QuitPlan { get; set; }
}
