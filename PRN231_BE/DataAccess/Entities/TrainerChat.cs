using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class TrainerChat
{
    [Key]
    public int ChatID { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserID { get; set; }

    [Required]
    [ForeignKey("Trainer")]
    public int TrainerID { get; set; }

    public string Message { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.Now;

    public bool Status { get; set; } = true;

    // Navigation Properties
    public User User { get; set; }
    public Trainer Trainer { get; set; }
}
