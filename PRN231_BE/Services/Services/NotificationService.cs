using System.Net;
using DataAccess.Common;
using DataAccess.Entities;
using DataAccess.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.IRepositories;
using Services.IServices;

namespace Services.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApiResponse> SendDailyNotificationsAsync()
    {
        var response = new ApiResponse();
        try
        {
            _logger.LogInformation("Starting daily notifications sending process");

            var customers = await _unitOfWork.Customers.GetAllAsync(
                c => c.IsDailyReminderEnabled,
                c => c.Statistics,
                c => c.DailyProgresses
            );

            var notificationsSent = 0;

            foreach (var customer in customers)
            {
                try
                {
                    await SendDailyReminderNotification(customer);
                    await CheckAndSendMilestoneNotifications(customer);
                    notificationsSent++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Failed to send notifications to customer {CustomerId}: {Error}",
                        customer.Id, ex.Message);
                }
            }

            await _unitOfWork.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = new { NotificationsSent = notificationsSent };
            response.ErrorMessages.Add($"Đã gửi thông báo hàng ngày cho {notificationsSent} khách hàng");

            _logger.LogInformation("Successfully sent daily notifications to {Count} customers", notificationsSent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending daily notifications");
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi gửi thông báo hàng ngày: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> SendMilestoneNotificationAsync(int customerId, int milestone)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy khách hàng");
                return response;
            }

            var notification = new Notification
            {
                CustomerId = customerId,
                Type = NotificationType.Milestone,
                Title = $"🎉 Chúc mừng! {milestone} ngày không hút thuốc!",
                Message = GetMilestoneMessage(milestone),
                IsRead = false,
                ScheduledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            response.Result = notification;
            response.ErrorMessages.Add("Gửi thông báo milestone thành công");

            _logger.LogInformation("Sent milestone notification to customer {CustomerId} for {Milestone} days",
                customerId, milestone);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending milestone notification to customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi gửi thông báo milestone: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> SendCustomNotificationAsync(int customerId, string title, string message)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy khách hàng");
                return response;
            }

            var notification = new Notification
            {
                CustomerId = customerId,
                Type = NotificationType.System,
                Title = title,
                Message = message,
                IsRead = false,
                ScheduledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.Created;
            response.Result = notification;
            response.ErrorMessages.Add("Gửi thông báo tùy chỉnh thành công");

            _logger.LogInformation("Sent custom notification to customer {CustomerId}", customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending custom notification to customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi gửi thông báo tùy chỉnh: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> GetCustomerNotificationsAsync(int customerId, int pageNumber = 1, int pageSize = 10)
    {
        var response = new ApiResponse();
        try
        {
            var customer = await _unitOfWork.Customers
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy khách hàng");
                return response;
            }

            var notifications = await _unitOfWork.Notifications
                .GetAllAsync(n => n.CustomerId == customerId);

            var totalCount = notifications.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var paginatedNotifications = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    Type = n.Type.ToString(),
                    n.IsRead,
                    n.ReadAt,
                    n.CreatedAt,
                    n.ScheduledAt,
                    n.SentAt
                })
                .ToList();

            var result = new
            {
                Notifications = paginatedNotifications,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1
            };

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = result;
            response.ErrorMessages.Add("Lấy danh sách thông báo thành công");

            _logger.LogInformation("Retrieved {Count} notifications for customer {CustomerId}",
                paginatedNotifications.Count, customerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for customer {CustomerId}", customerId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi lấy danh sách thông báo: {ex.Message}");
        }

        return response;
    }

    public async Task<ApiResponse> MarkNotificationAsReadAsync(int notificationId)
    {
        var response = new ApiResponse();
        try
        {
            var notification = await _unitOfWork.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification == null)
            {
                response.IsSuccess = false;
                response.StatusCode = HttpStatusCode.NotFound;
                response.ErrorMessages.Add("Không tìm thấy thông báo");
                return response;
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveAsync();

            response.IsSuccess = true;
            response.StatusCode = HttpStatusCode.OK;
            response.Result = notification;
            response.ErrorMessages.Add("Đánh dấu thông báo đã đọc thành công");

            _logger.LogInformation("Marked notification {NotificationId} as read", notificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
            response.IsSuccess = false;
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.ErrorMessages.Add($"Lỗi đánh dấu thông báo đã đọc: {ex.Message}");
        }

        return response;
    }

    private async Task SendDailyReminderNotification(Customer customer)
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
                ScheduledAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
        }
    }

    private async Task CheckAndSendMilestoneNotifications(Customer customer)
    {
        if (customer.Statistics == null) return;

        var currentStreak = customer.Statistics.CurrentStreak;
        var milestones = new[] { 1, 3, 7, 14, 30, 60, 90, 180, 365 };

        if (milestones.Contains(currentStreak))
        {
            // Kiểm tra xem đã gửi milestone notification cho ngày này chưa
            var today = DateTime.Today;
            var existingMilestone = await _unitOfWork.Notifications
                .FirstOrDefaultAsync(n =>
                    n.CustomerId == customer.Id &&
                    n.Type == NotificationType.Milestone &&
                    n.CreatedAt.Date == today);

            if (existingMilestone == null)
            {
                var notification = new Notification
                {
                    CustomerId = customer.Id,
                    Type = NotificationType.Milestone,
                    Title = $"🎉 Chúc mừng! {currentStreak} ngày không hút thuốc!",
                    Message = GetMilestoneMessage(currentStreak),
                    IsRead = false,
                    ScheduledAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Notifications.AddAsync(notification);
            }
        }
    }

    private string GetMilestoneMessage(int days)
    {
        return days switch
        {
            1 => "Ngày đầu tiên thành công! Đây là bước khởi đầu tuyệt vời! 🎯",
            3 => "3 ngày! Cơ thể bạn đang bắt đầu phục hồi! 💪",
            7 => "1 tuần! Bạn đã vượt qua giai đoạn khó khăn nhất! 🏆",
            14 => "2 tuần! Hệ tuần hoàn của bạn đang cải thiện! ❤️",
            30 => "1 tháng! Phổi bạn đang tự làm sạch! 🫁",
            60 => "2 tháng! Nguy cơ nhiễm trùng đã giảm đáng kể! 🛡️",
            90 => "3 tháng! Chức năng phổi tăng 30%! 📈",
            180 => "6 tháng! Nguy cơ bệnh tim giảm một nửa! 🫶",
            365 => "1 năm! Nguy cơ bệnh tim chỉ bằng một nửa so với người hút thuốc! 🎊",
            _ => $"Thật tuyệt vời! {days} ngày là một thành tích đáng tự hào! ⭐"
        };
    }
}