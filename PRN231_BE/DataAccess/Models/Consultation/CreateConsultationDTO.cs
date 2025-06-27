namespace DataAccess.Models.Consultation;

public class CreateConsultationDTO
{
    public int CoachId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? Notes { get; set; }
} 