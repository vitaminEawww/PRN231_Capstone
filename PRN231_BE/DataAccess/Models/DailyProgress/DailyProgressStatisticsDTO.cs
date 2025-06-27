namespace DataAccess.Models.DailyProgress;

public class DailyProgressStatisticsDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;

    // Thống kê thời gian
    public int TotalDaysTracked { get; set; }
    public int SmokeFreeeDays { get; set; }
    public int SmokingDays { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateTime? LastSmokingDate { get; set; }

    // Thống kê thuốc lá và tiền bạc
    public int TotalCigarettesSmoked { get; set; }
    public int TotalCigarettesAvoided { get; set; }
    public decimal TotalMoneySpent { get; set; }
    public decimal TotalMoneySaved { get; set; }
    public decimal AverageDailyCost { get; set; }
    public decimal AverageDailySaving { get; set; }

    // Thống kê sức khỏe
    public float AverageCravingLevel { get; set; }
    public float AverageMoodLevel { get; set; }
    public float AverageEnergyLevel { get; set; }

    // Xu hướng cải thiện (so với 7 ngày trước)
    public float CravingTrend { get; set; }
    public float MoodTrend { get; set; }
    public float EnergyTrend { get; set; }

    // Ngày cập nhật thống kê
    public DateTime LastCalculated { get; set; }

    // Progress visualization data
    public List<DailyProgressSummaryDTO> RecentProgress { get; set; } = new();
}

public class DailyProgressSummaryDTO
{
    public DateTime Date { get; set; }
    public bool IsSmokeFree { get; set; }
    public int CigarettesSmoked { get; set; }
    public decimal MoneySpent { get; set; }
    public int CravingLevel { get; set; }
    public int MoodLevel { get; set; }
    public int EnergyLevel { get; set; }
}