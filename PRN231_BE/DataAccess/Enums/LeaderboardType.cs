using System;

namespace DataAccess.Enums;

/// <summary>
/// Loại bảng xếp hạng
/// </summary>
public enum LeaderboardType
{
    SmokeFreesDays = 1,        // Số ngày không hút thuốc
    CurrentStreak = 2,         // Chuỗi ngày hiện tại
    LongestStreak = 3,         // Chuỗi ngày dài nhất
    MoneySaved = 4,            // Tiền tiết kiệm được
    CigarettesAvoided = 5,     // Số điếu tránh được
    TotalPoints = 6,           // Tổng điểm tích lũy
    TotalBadges = 7,           // Số huy hiệu đạt được
    CommunityEngagement = 8,   // Tương tác cộng đồng
    HealthImprovement = 9,     // Cải thiện sức khỏe
    ConsistencyScore = 10      // Điểm kiên trì
}
