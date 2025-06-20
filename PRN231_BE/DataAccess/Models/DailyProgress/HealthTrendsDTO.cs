using System;
using DataAccess.Enums;

namespace DataAccess.Models.DailyProgress;

public class HealthTrendsDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public int Days { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Xu hướng tổng quan
    public HealthTrendSummary CravingTrend { get; set; } = new();
    public HealthTrendSummary MoodTrend { get; set; } = new();
    public HealthTrendSummary EnergyTrend { get; set; } = new();

    // Dữ liệu biểu đồ
    public List<DailyHealthMetric> DailyMetrics { get; set; } = new();

    // Insight và recommendations
    public List<string> Insights { get; set; } = new();
    public List<string> Recommendations { get; set; } = new();
}

public class HealthTrendSummary
{
    public float CurrentAverage { get; set; }
    public float PreviousAverage { get; set; }
    public float Change { get; set; }
    public string ChangePercent { get; set; } = null!;
    public TrendDirection Direction { get; set; }
    public string DirectionText => Direction.ToString();
}

public class DailyHealthMetric
{
    public DateTime Date { get; set; }
    public int CravingLevel { get; set; }
    public int MoodLevel { get; set; }
    public int EnergyLevel { get; set; }
    public bool IsSmokeFree { get; set; }
}
