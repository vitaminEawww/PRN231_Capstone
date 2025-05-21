using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Membership
{
    [Key]
    public int PlanID { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Duration { get; set; }

    public bool Status { get; set; } = true;
}
