using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Repositories.IRepositories;

namespace Services.Services;

public class NotificationBackgroundService(
    IServiceProvider serviceProvider,
    IUnitOfWork unitOfWork)
    : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessNotificationsAsync();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error processing notifications: {ex.Message}");
            }

            // Wait for 1 minute before next iteration
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessNotificationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var users = await _unitOfWork.Notifications.GetAllAsync();

        foreach (var user in users)
        {
            var preferences = await notificationService.GetUserNotificationPreferencesAsync(user.Id);
            if (preferences == null) continue;

            var now = DateTime.UtcNow;

            // Process daily reminders
            if (preferences.IsDailyReminderEnabled &&
                preferences.DailyReminderTime.HasValue &&
                now.TimeOfDay.Hours == preferences.DailyReminderTime.Value.Hours &&
                now.TimeOfDay.Minutes == preferences.DailyReminderTime.Value.Minutes)
            {
                await notificationService.SendDailyReminderAsync(user.Id);
            }

            // Process weekly reports
            if (preferences.IsWeeklyReportEnabled &&
                preferences.WeeklyReportDay.HasValue &&
                now.DayOfWeek == preferences.WeeklyReportDay.Value &&
                now.Hour == 9 && now.Minute == 0) // Send at 9 AM
            {
                await notificationService.SendWeeklyReportAsync(user.Id);
            }

            // Send motivational messages randomly (once per day)
            if (preferences.IsMotivationalMessageEnabled &&
                now.Hour == 14 && now.Minute == 0) // Send at 2 PM
            {
                await notificationService.SendMotivationalMessageAsync(user.Id);
            }
        }
    }
}