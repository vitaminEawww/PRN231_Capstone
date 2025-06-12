using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Enums;

namespace DataAccess.Entities;

public class DailyLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int DailyTaskId { get; set; }

    [Required]
    public DateTime LogDate { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsCompleted { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; }

    public int? MoodRating { get; set; }  // Scale of 1-5

    public int? CravingsCount { get; set; }

    public int? CigarettesSmoked { get; set; }

    public decimal? MoneySaved { get; set; }

    public DailyLogStatus Status { get; set; } = DailyLogStatus.NotStarted;

    public int? MinutesOfExercise { get; set; }  // Track exercise minutes for the day

    public int? WaterIntake { get; set; }  // Track water intake in ml

    public bool? UsedNicotineReplacement { get; set; }  // Track if nicotine replacement was used

    // Health Tracking Fields
    public decimal? Weight { get; set; }  // Weight in kg
    public int? SleepHours { get; set; }  // Hours of sleep
    public int? StressLevel { get; set; }  // Stress level 1-10
    public int? HeartRate { get; set; }  // Heart rate in bpm
    public int? SystolicBP { get; set; }  // Systolic blood pressure
    public int? DiastolicBP { get; set; }  // Diastolic blood pressure
    [MaxLength(500)]
    public string HealthNotes { get; set; }  // Additional health notes

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [ForeignKey("DailyTaskId")]
    public virtual DailyTask DailyTask { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}