using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class DailyTask
{
    [Key]
    public int ProgressID { get; set; }

    [Required]
    public int UserID { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int CigarettesAvoided { get; set; }

    public decimal MoneySaved { get; set; }

    public bool IsCompleted { get; set; } = true;

    public DailyLog DailyLog { get; set; }
}
