namespace DataAccess.Models.Notifications;

public class NotificationPreferences
{
    public int CustomerId { get; set; }
    public bool IsNotificationEnabled { get; set; }
    public bool IsDailyReminderEnabled { get; set; }
    public bool IsWeeklyReportEnabled { get; set; }
    public bool IsMotivationalMessageEnabled { get; set; }
    public bool IsQuitPlanReminderEnabled { get; set; }
    public TimeSpan? DailyReminderTime { get; set; }
    public DayOfWeek? WeeklyReportDay { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}