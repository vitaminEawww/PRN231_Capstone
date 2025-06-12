using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

/// <summary>
/// Buổi tư vấn
/// </summary>
public partial class Consultation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int CoachId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public ConsultationStatus Status { get; set; }
    public ConsultationType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public int? Rating { get; set; }
    public string? Feedback { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Coach? Coach { get; set; }
}
