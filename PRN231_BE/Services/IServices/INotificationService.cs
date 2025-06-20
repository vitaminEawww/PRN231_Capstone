using System;
using DataAccess.Common;

namespace Services.IServices;

public interface INotificationService
{
    Task<ApiResponse> SendDailyNotificationsAsync();
    Task<ApiResponse> SendMilestoneNotificationAsync(int customerId, int milestone);
    Task<ApiResponse> SendCustomNotificationAsync(int customerId, string title, string message);
    Task<ApiResponse> GetCustomerNotificationsAsync(int customerId, int pageNumber = 1, int pageSize = 10);
    Task<ApiResponse> MarkNotificationAsReadAsync(int notificationId);
}
