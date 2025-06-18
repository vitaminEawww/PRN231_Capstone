using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Models.Notifications;

namespace WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications([FromQuery] bool includeRead = false)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value);
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, includeRead);
        return Ok(notifications);
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkNotificationAsReadAsync(id);
        return Ok();
    }

    [HttpGet("preferences")]
    public async Task<ActionResult<NotificationPreferences>> GetPreferences()
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value);
        var preferences = await _notificationService.GetUserNotificationPreferencesAsync(userId);
        return Ok(preferences);
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] NotificationPreferences preferences)
    {
        var userId = int.Parse(User.FindFirst("sub")?.Value);
        preferences.CustomerId = userId;
        await _notificationService.UpdateNotificationPreferencesAsync(preferences);
        return Ok();
    }
}