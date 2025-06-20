namespace DataAccess.Models.DailyProgress;

public class MonthlyProgressSummaryDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = null!;
    public int DaysInMonth { get; set; }

    // Thống kê tổng quan
    public int SmokeFreeeDaysCount { get; set; }
    public int SmokingDaysCount { get; set; }
    public double SmokeFreePercentage { get; set; }

    // Thống kê thuốc lá và tiền bạc
    public int TotalCigarettesSmoked { get; set; }
    public int CigarettesAvoided { get; set; }
    public decimal TotalMoneySpent { get; set; }
    public decimal MoneySaved { get; set; }

    // Streak thông tin
    public int BestStreak { get; set; }
    public int CurrentMonthStreak { get; set; }

    // Sức khỏe
    public float AverageCravingLevel { get; set; }
    public float AverageMoodLevel { get; set; }
    public float AverageEnergyLevel { get; set; }

    // Phân tích theo tuần
    public List<WeeklyProgressSummaryDTO> WeeklyBreakdown { get; set; } = new();

    // So sánh với tháng trước
    public MonthComparison Comparison { get; set; } = new();

    // Top achievements
    public List<string> MonthlyAchievements { get; set; } = new();
}

public class MonthComparison
{
    public int SmokeFreeeDaysChange { get; set; }
    public double SmokeFreePercentageChange { get; set; }
    public decimal MoneySavedChange { get; set; }
    public float HealthScoreChange { get; set; }
}