using EADProject.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // Get unread notifications for CSR admin.
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var notifications = await _notificationService.GetUnreadNotificationsAsync();
        return Ok(notifications);
    }

    // Mark a notification as read.
    [HttpPut("markAsRead/{id}")]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        await _notificationService.MarkNotificationAsReadAsync(id);
        return Ok("Notification marked as read.");
    }
}
