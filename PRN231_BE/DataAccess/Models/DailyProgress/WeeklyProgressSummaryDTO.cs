namespace DataAccess.Models.DailyProgress;

public class WeeklyProgressSummaryDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public DateTime WeekStartDate { get; set; }
    public DateTime WeekEndDate { get; set; }
    public string WeekLabel { get; set; } = null!;

    // Thống kê cơ bản
    public int SmokeFreeeDays { get; set; }
    public int SmokingDays { get; set; }
    public int TotalCigarettesSmoked { get; set; }
    public decimal TotalMoneySpent { get; set; }
    public decimal MoneySaved { get; set; }

    // Trung bình sức khỏe
    public float AverageCravingLevel { get; set; }
    public float AverageMoodLevel { get; set; }
    public float AverageEnergyLevel { get; set; }

    // So sánh với tuần trước
    public WeekComparison Comparison { get; set; } = new();

    // Chi tiết từng ngày
    public List<DailyProgressSummaryDTO> DailyDetails { get; set; } = new();

    // Achievements
    public List<string> Achievements { get; set; } = new();
}

public class WeekComparison
{
    public int SmokeFreeeDaysChange { get; set; }
    public int CigarettesChange { get; set; }
    public decimal MoneySpentChange { get; set; }
    public float CravingLevelChange { get; set; }
    public float MoodLevelChange { get; set; }
    public float EnergyLevelChange { get; set; }
}