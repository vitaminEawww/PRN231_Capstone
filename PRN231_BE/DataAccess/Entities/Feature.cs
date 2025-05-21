using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class Feature
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }
}