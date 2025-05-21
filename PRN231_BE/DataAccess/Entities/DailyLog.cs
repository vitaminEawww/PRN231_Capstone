using System.ComponentModel.DataAnnotations;
using DataAccess.Enums;

namespace DataAccess.Entities;

public class DailyLog
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PhaseId { get; set; }

    public DateTime LogDate { get; set; } = DateTime.UtcNow;

    public int CigarettesSmokedCount { get; set; }

    public int CravingsCount { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; }

    public int MoodRating { get; set; }  // Scale of 1-10

    public bool CompletedDailyChallenge { get; set; }

    public DailyLogStatus Status { get; set; } = DailyLogStatus.NotStarted;
}