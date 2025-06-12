using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Enums;

namespace DataAccess.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Message { get; set; }

    [Required]
    [MaxLength(50)]
    public string Type { get; set; }  // "Reminder", "Achievement", "Plan", "Community", "System"

    public bool IsRead { get; set; } = false;

    public bool IsScheduled { get; set; } = false;

    public DateTime? ScheduledAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(1000)]
    public string Data { get; set; }  // JSON data for extra info

    public int Priority { get; set; } = 1;  // 1=Low, 2=Medium, 3=High

    public DateTime? ExpiresAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
} 