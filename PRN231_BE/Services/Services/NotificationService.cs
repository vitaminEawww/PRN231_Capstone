using Services.Interfaces;
using DataAccess.Models.Notifications;
using Repositories.IRepositories;
using Mapster;
using DataAccess.Entities;
using DataAccess.Enums;
using System.Linq.Expressions;

namespace Services.Services;

public class NotificationService(
    IUnitOfWork unitOfWork,
    IFirebaseService firebaseService) : INotificationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFirebaseService _firebaseService = firebaseService;
    private readonly DateTime uctNow = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(7), DateTimeKind.Unspecified);

    public async Task<NotificationResponseDTO> CreateNotificationAsync(NotificationDTO notification)
    {
        var entity = notification.Adapt<Notification>();
        entity.CreatedAt = DateTime.UtcNow;
        entity.ScheduledAt = DateTime.UtcNow;
        await _unitOfWork.Notifications.AddAsync(entity);

        // Get user's FCM token
        var user = await _unitOfWork.Users.GetByIdAsync(notification.CustomerId);
        if (!string.IsNullOrEmpty(user?.FcmToken))
        {
            // Send push notification
            await _firebaseService.SendNotificationAsync(
                user.FcmToken,
                notification.Title,
                notification.Message,
                new Dictionary<string, string>
                {
                    { "type", notification.Type.ToString() },
                    { "notificationId", entity.Id.ToString() }
                });
        }

        return entity.Adapt<NotificationResponseDTO>();
    }

    public async Task<IEnumerable<NotificationResponseDTO>> GetUserNotificationsAsync(int customerId, bool includeRead = false)
    {
        Expression<Func<Notification, bool>> filter = n => n.CustomerId == customerId;
        if (!includeRead)
        {
            filter = n => n.CustomerId == customerId && !n.IsRead;
        }

        var notifications = await _unitOfWork.Notifications.GetAllAsync(filter);
        return notifications.Adapt<IEnumerable<NotificationResponseDTO>>();
    }

    public async Task MarkNotificationAsReadAsync(int notificationId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            notification.ReadAt = uctNow;
            _unitOfWork.Notifications.Update(notification);
        }
    }

    public async Task<NotificationPreferences> GetUserNotificationPreferencesAsync(int customerId)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
        if (customer == null) return null;

        return new NotificationPreferences
        {
            CustomerId = customerId,
            IsNotificationEnabled = customer.IsNotificationEnabled,
            IsDailyReminderEnabled = customer.IsDailyReminderEnabled,
            IsWeeklyReportEnabled = customer.IsWeeklyReportEnabled,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }

    public async Task UpdateNotificationPreferencesAsync(NotificationPreferences preferences)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(preferences.CustomerId);
        if (customer == null) return;

        customer.IsNotificationEnabled = preferences.IsNotificationEnabled;
        customer.IsDailyReminderEnabled = preferences.IsDailyReminderEnabled;
        customer.IsWeeklyReportEnabled = preferences.IsWeeklyReportEnabled;
        customer.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Customers.Update(customer);
    }

    public async Task SendDailyReminderAsync(int customerId)
    {
        var preferences = await GetUserNotificationPreferencesAsync(customerId);
        if (preferences?.IsDailyReminderEnabled == true && preferences.IsNotificationEnabled)
        {
            var notification = new NotificationDTO
            {
                CustomerId = customerId,
                Type = NotificationType.DailyReminder,
                Title = "Daily Reminder",
                Message = "Don't forget to track your progress today! Stay strong in your journey to quit smoking.",
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = DateTime.UtcNow
            };
            await CreateNotificationAsync(notification);
        }
    }

    public async Task SendWeeklyReportAsync(int customerId)
    {
        var preferences = await GetUserNotificationPreferencesAsync(customerId);
        if (preferences?.IsWeeklyReportEnabled == true && preferences.IsNotificationEnabled)
        {
            var notification = new NotificationDTO
            {
                CustomerId = customerId,
                Type = NotificationType.WeeklyReport,
                Title = "Weekly Progress Report",
                Message = "Here's your weekly progress report. Keep up the good work!",
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = DateTime.UtcNow
            };
            await CreateNotificationAsync(notification);
        }
    }

    public async Task SendMotivationalMessageAsync(int customerId)
    {
        var preferences = await GetUserNotificationPreferencesAsync(customerId);
        if (preferences?.IsNotificationEnabled == true)
        {
            var motivationalMessages = new[]
            {
                "Every day without smoking is a victory!",
                "Your health is improving with each smoke-free day.",
                "You're stronger than your cravings!",
                "Think of all the money you're saving!",
                "Your future self thanks you for quitting smoking."
            };

            var random = new Random();
            var message = motivationalMessages[random.Next(motivationalMessages.Length)];

            var notification = new NotificationDTO
            {
                CustomerId = customerId,
                Type = NotificationType.Motivation,
                Title = "Motivational Message",
                Message = message,
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = DateTime.UtcNow
            };
            await CreateNotificationAsync(notification);
        }
    }

    public async Task SendQuitPlanReminderAsync(int customerId, string reason)
    {
        var preferences = await GetUserNotificationPreferencesAsync(customerId);
        if (preferences?.IsNotificationEnabled == true)
        {
            var notification = new NotificationDTO
            {
                CustomerId = customerId,
                Type = NotificationType.Motivation,
                Title = "Remember Your Why",
                Message = $"Remember why you started: {reason}",
                CreatedAt = DateTime.UtcNow,
                ScheduledAt = DateTime.UtcNow
            };
            await CreateNotificationAsync(notification);
        }
    }
}