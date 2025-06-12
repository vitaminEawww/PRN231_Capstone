using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

/// <summary>
/// Tổng hợp đánh giá cho mỗi đối tượng - Cache để tăng performance
/// </summary>
public class RatingSummary
{
    public int Id { get; set; }
    public RatingTargetType TargetType { get; set; }
    public int TargetId { get; set; }
    public float AverageScore { get; set; } = 0;
    public int TotalRatings { get; set; } = 0;
    public int FiveStarCount { get; set; } = 0;
    public int FourStarCount { get; set; } = 0;
    public int ThreeStarCount { get; set; } = 0;
    public int TwoStarCount { get; set; } = 0;
    public int OneStarCount { get; set; } = 0;
    public DateTime LastUpdated { get; set; }

    // Calculated Properties (không lưu DB)
    public double FiveStarPercentage => TotalRatings > 0 ? (double)FiveStarCount / TotalRatings * 100 : 0;
    public double FourStarPercentage => TotalRatings > 0 ? (double)FourStarCount / TotalRatings * 100 : 0;
    public double ThreeStarPercentage => TotalRatings > 0 ? (double)ThreeStarCount / TotalRatings * 100 : 0;
    public double TwoStarPercentage => TotalRatings > 0 ? (double)TwoStarCount / TotalRatings * 100 : 0;
    public double OneStarPercentage => TotalRatings > 0 ? (double)OneStarCount / TotalRatings * 100 : 0;
}
