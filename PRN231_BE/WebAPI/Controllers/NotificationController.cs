using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.IServices;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ICustomerService _customerService;

    public NotificationController(
        INotificationService notificationService,
        ICustomerService customerService)
    {
        _notificationService = notificationService;
        _customerService = customerService;
    }

    /// <summary>
    /// Lấy danh sách thông báo của customer hiện tại
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var customerId = await _customerService.GetCustomerIdByUserId(userId);

        var result = await _notificationService.GetCustomerNotificationsAsync(customerId, pageNumber, pageSize);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Đánh dấu thông báo đã đọc
    /// </summary>
    [HttpPut("{notificationId}/mark-read")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        var result = await _notificationService.MarkNotificationAsReadAsync(notificationId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Gửi thông báo tùy chỉnh cho customer (Admin only)
    /// </summary>
    [HttpPost("send-custom")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendCustomNotification(
        [FromBody] SendCustomNotificationRequest request)
    {
        var result = await _notificationService.SendCustomNotificationAsync(
            request.CustomerId, request.Title, request.Message);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Gửi thông báo milestone cho customer (Admin only)
    /// </summary>
    [HttpPost("send-milestone")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendMilestoneNotification(
        [FromBody] SendMilestoneNotificationRequest request)
    {
        var result = await _notificationService.SendMilestoneNotificationAsync(
            request.CustomerId, request.Milestone);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Gửi thông báo hàng ngày cho tất cả customers (Admin only)
    /// </summary>
    [HttpPost("send-daily")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendDailyNotifications()
    {
        var result = await _notificationService.SendDailyNotificationsAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}

public class SendCustomNotificationRequest
{
    public int CustomerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SendMilestoneNotificationRequest
{
    public int CustomerId { get; set; }
    public int Milestone { get; set; }
}