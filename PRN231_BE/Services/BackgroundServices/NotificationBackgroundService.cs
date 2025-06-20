using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;

namespace Services.BackgroundServices;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(24); // Chạy mỗi ngày

    public NotificationBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotificationBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Đợi đến 9:00 AM mỗi ngày
        await WaitUntilDailyTime(9, 0);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendDailyNotifications();

                // Đợi đến 9:00 AM ngày hôm sau
                await WaitUntilDailyTime(9, 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing notification background service");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

    private async Task SendDailyNotifications()
    {
        _logger.LogInformation("Starting daily notification sending");

        using var scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            // Lấy tất cả customers có bật notification
            var customers = await unitOfWork.Customers.GetAllAsync(c =>
                c.IsDailyReminderEnabled,
                c => c.Statistics,
                c => c.DailyProgresses);

            foreach (var customer in customers)
            {
                await SendDailyReminderNotification(unitOfWork, customer);
                await CheckMilestoneNotifications(unitOfWork, customer);
            }

            await unitOfWork.SaveAsync();
            _logger.LogInformation("Successfully sent daily notifications to {Count} customers", customers.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send daily notifications");
            throw;
        }
    }

    private async Task SendDailyReminderNotification(IUnitOfWork unitOfWork, Customer customer)
    {
        var today = DateTime.Today;
        var hasLoggedToday = customer.DailyProgresses.Any(dp => dp.Date.Date == today);

        if (!hasLoggedToday)
        {
            var notification = new Notification
            {
                CustomerId = customer.Id,
                Type = NotificationType.DailyReminder,
                Title = "Đừng quên ghi chép hôm nay! 📝",
                Message = "Hãy ghi lại tiến trình cai thuốc của bạn hôm nay để theo dõi sự tiến bộ.",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Notifications.AddAsync(notification);
        }
    }

    private async Task CheckMilestoneNotifications(IUnitOfWork unitOfWork, Customer customer)
    {
        if (customer.Statistics == null) return;

        var currentStreak = customer.Statistics.CurrentStreak;

        // Kiểm tra milestone
        var milestones = new[] { 1, 3, 7, 14, 30, 60, 90, 180, 365 };

        if (milestones.Contains(currentStreak))
        {
            var notification = new Notification
            {
                CustomerId = customer.Id,
                Type = NotificationType.Milestone,
                Title = $"🎉 Chúc mừng! {currentStreak} ngày không hút thuốc!",
                Message = GetMilestoneMessage(currentStreak),
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Notifications.AddAsync(notification);
        }
    }

    private string GetMilestoneMessage(int days)
    {
        return days switch
        {
            1 => "Ngày đầu tiên thành công! Đây là bước khởi đầu tuyệt vời!",
            3 => "3 ngày! Cơ thể bạn đang bắt đầu phục hồi!",
            7 => "1 tuần! Bạn đã vượt qua giai đoạn khó khăn nhất!",
            14 => "2 tuần! Hệ tuần hoàn của bạn đang cải thiện!",
            30 => "1 tháng! Phổi bạn đang tự làm sạch!",
            60 => "2 tháng! Nguy cơ nhiễm trùng đã giảm đáng kể!",
            90 => "3 tháng! Chức năng phổi tăng 30%!",
            180 => "6 tháng! Nguy cơ bệnh tim giảm một nửa!",
            365 => "1 năm! Nguy cơ bệnh tim chỉ bằng một nửa so với người hút thuốc!",
            _ => $"Thật tuyệt vời! {days} ngày là một thành tích đáng tự hào!"
        };
    }

    private async Task WaitUntilDailyTime(int hour, int minute)
    {
        var now = DateTime.Now;
        var targetTime = DateTime.Today.AddHours(hour).AddMinutes(minute);

        if (now > targetTime)
        {
            targetTime = targetTime.AddDays(1);
        }

        var delay = targetTime - now;
        if (delay.TotalMilliseconds > 0)
        {
            await Task.Delay(delay);
        }
    }
}