using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class Phase
{
    [Key]
    public int Id { get; set; }

    public int PlanId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int OrderNumber { get; set; }

    public int DurationDays { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime EndDate { get; set; }

    public double IsCompleted { get; set; }

    // Navigation properties
    [ForeignKey("PlanId")]
    public virtual Plan Plan { get; set; }

    public virtual ICollection<DailyTask> DailyTasks { get; set; }
}