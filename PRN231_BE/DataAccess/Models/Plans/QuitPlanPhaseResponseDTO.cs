namespace DataAccess.Models.Plans;

public class QuitPlanPhaseResponseDTO
{
    public int Id { get; set; }
    public int QuitPlanId { get; set; }
    public int PhaseNumber { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TargetCigarettesPerDay { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Thêm thông tin hữu ích
    public bool IsActive { get; set; }
    public bool IsUpcoming { get; set; }
    public int DaysRemaining { get; set; }
    public int TotalDays { get; set; }
}