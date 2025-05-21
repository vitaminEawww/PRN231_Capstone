using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Plan
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int DurationDays { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    // Navigation properties
    public virtual ICollection<UserPlan> UserPlans { get; set; }
    public virtual ICollection<Phase> Phases { get; set; }
}