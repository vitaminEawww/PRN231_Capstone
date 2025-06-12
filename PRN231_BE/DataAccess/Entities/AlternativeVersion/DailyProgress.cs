using System;

namespace DataAccess.Entities.AlternativeVersion;

public class DailyProgress
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public int CigarettesSmoked { get; set; } = 0;
    public decimal MoneySpent { get; set; } = 0;
    public int CravingLevel { get; set; } = 0; // 1-10 - mức độ thèm muốn
    public int MoodLevel { get; set; } = 0; // 1-10
    public int EnergyLevel { get; set; } = 0; // 1-10
    public string? Notes { get; set; }
    public string? Triggers { get; set; } //* Tác nhân gây thèm thuốc
    public bool IsSmokeFree { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
