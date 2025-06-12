using System;

namespace DataAccess.Entities.AlternativeVersion;

/// <summary>
/// Thống kê tổng hợp của từng khách hàng - Dùng để tính leaderboard
/// </summary>
public class CustomerStatistics
{
    public int Id { get; set; }
    public int CustomerId { get; set; }

    //* Thống kê cai thuốc
    public int TotalSmokeFreesDays { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0; // Chuỗi ngày hiện tại không hút
    public int LongestStreak { get; set; } = 0; // Chuỗi dài nhất từng đạt được
    public DateTime? LastSmokingDate { get; set; }

    //* Thống kê tiền bạc
    public decimal TotalMoneySaved { get; set; } = 0;
    public decimal AverageDailySaving { get; set; } = 0;

    //* Thống kê thuốc lá
    public int TotalCigarettesAvoided { get; set; } = 0;
    public int TotalPacksAvoided { get; set; } = 0;

    //* Thống kê sức khỏe (từ DailyProgress)
    public float AverageMoodLevel { get; set; } = 0;
    public float AverageEnergyLevel { get; set; } = 0;
    public float AverageCravingLevel { get; set; } = 0;

    //* Thống kê hoạt động
    public int TotalPosts { get; set; } = 0;
    public int TotalLikesReceived { get; set; } = 0;
    public int TotalCommentsReceived { get; set; } = 0;
    public int TotalBadgesEarned { get; set; } = 0;
    public int TotalPoints { get; set; } = 0;

    //* Meta data
    public DateTime LastCalculated { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
