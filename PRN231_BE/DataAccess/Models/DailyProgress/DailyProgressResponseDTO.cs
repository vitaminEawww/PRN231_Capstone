namespace DataAccess.Models.DailyProgress;

public class DailyProgressResponseDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateTime Date { get; set; }
    public int CigarettesSmoked { get; set; }
    public decimal MoneySpent { get; set; }
    public int CravingLevel { get; set; }
    public int MoodLevel { get; set; }
    public int EnergyLevel { get; set; }
    public string? Notes { get; set; }
    public string? Triggers { get; set; }
    public bool IsSmokeFree { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Calculated fields
    public string CravingLevelText => GetLevelText(CravingLevel);
    public string MoodLevelText => GetLevelText(MoodLevel);
    public string EnergyLevelText => GetLevelText(EnergyLevel);
    public string DateText => Date.ToString("dd/MM/yyyy");

    private string GetLevelText(int level)
    {
        return level switch
        {
            0 => "Không có",
            1 or 2 => "Rất thấp",
            3 or 4 => "Thấp",
            5 or 6 => "Trung bình",
            7 or 8 => "Cao",
            9 or 10 => "Rất cao",
            _ => "Chưa xác định"
        };
    }
}