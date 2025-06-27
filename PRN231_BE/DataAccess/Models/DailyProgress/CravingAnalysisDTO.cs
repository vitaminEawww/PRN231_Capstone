namespace DataAccess.Models.DailyProgress;

public class CravingAnalysisDTO
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public int Days { get; set; }
    public DateTime AnalysisPeriod { get; set; }

    // Phân tích tổng quan
    public float AverageCravingLevel { get; set; }
    public int HighCravingDays { get; set; } // >= 7
    public int ModerateCravingDays { get; set; } // 4-6
    public int LowCravingDays { get; set; } // 1-3
    public int NoCravingDays { get; set; } // 0

    // Phân tích theo thời gian
    public List<CravingByTimeOfDay> CravingByHour { get; set; } = new();
    public List<CravingByDayOfWeek> CravingByWeekday { get; set; } = new();

    // Top triggers
    public List<TriggerFrequency> TopTriggers { get; set; } = new();

    // Correlation với smoking
    public float CravingSmokingCorrelation { get; set; }
    public string CorrelationDescription { get; set; } = null!;

    // Recommendations
    public List<string> Recommendations { get; set; } = new();
}

public class CravingByTimeOfDay
{
    public int Hour { get; set; }
    public float AverageCraving { get; set; }
    public int DataPoints { get; set; }
}

public class CravingByDayOfWeek
{
    public DayOfWeek DayOfWeek { get; set; }
    public string DayName { get; set; } = null!;
    public float AverageCraving { get; set; }
    public int DataPoints { get; set; }
}

public class TriggerFrequency
{
    public string Trigger { get; set; } = null!;
    public int Count { get; set; }
    public double AverageCravingLevel { get; set; }
}