using System;
using DataAccess.Enums;

namespace DataAccess.Entities.AlternativeVersion;

public partial class Notification
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
}
