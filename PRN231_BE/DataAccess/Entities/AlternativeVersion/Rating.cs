using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

/// <summary>
/// Hệ thống đánh giá tổng quát
/// </summary>
public class Rating
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public RatingTargetType TargetType { get; set; }
    public int TargetId { get; set; }
    public int Score { get; set; } // 1-5 sao
    public string? Comment { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public RatingStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
