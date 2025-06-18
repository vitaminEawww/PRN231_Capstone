using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using DataAccess.Models.Notifications;

namespace Services.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDTO> CreateNotificationAsync(NotificationDTO notification);
        Task<IEnumerable<NotificationResponseDTO>> GetUserNotificationsAsync(int userId, bool includeRead = false);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task<NotificationPreferences> GetUserNotificationPreferencesAsync(int userId);
        Task UpdateNotificationPreferencesAsync(NotificationPreferences preferences);
        Task SendDailyReminderAsync(int userId);
        Task SendWeeklyReportAsync(int userId);
        Task SendMotivationalMessageAsync(int userId);
        Task SendQuitPlanReminderAsync(int userId, string reason);
    }
}