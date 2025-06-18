using DataAccess.Enums;

namespace DataAccess.Models.Notifications;

public class NotificationDTO
{
    // public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationResponseDTO
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
