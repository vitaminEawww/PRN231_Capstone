using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class DailyTask
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PhaseId { get; set; }

    [Required]
    public int DayNumber { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("PhaseId")]
    public virtual Phase Phase { get; set; }

    public virtual ICollection<DailyLog> DailyLogs { get; set; } = new List<DailyLog>();
}
