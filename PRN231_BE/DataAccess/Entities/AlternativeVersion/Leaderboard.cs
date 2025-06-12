using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

/// <summary>
/// Bảng xếp hạng - Cache kết quả để tăng performance
/// </summary>
public partial class Leaderboard
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public LeaderboardType Type { get; set; }
    public LeaderboardPeriod Period { get; set; }
    public int Score { get; set; }
    public int Rank { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual Customer? Customer { get; set; }
}
