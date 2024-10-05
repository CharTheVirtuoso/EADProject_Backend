/***************************************************************
 * File Name: AdminNotificationService.cs
 * Description: Represents the data service for notifications for admin
 * Date Created: September 15, 2024
 ***************************************************************/
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/admin-notifications")]
[ApiController]
public class AdminNotificationController : ControllerBase
{
    private readonly AdminNotificationService _adminNotificationService;

    public AdminNotificationController(AdminNotificationService adminNotificationService)
    {
        _adminNotificationService = adminNotificationService;
    }

    // Get unread admin notifications
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadAdminNotifications()
    {
        var notifications = await _adminNotificationService.GetUnreadAdminNotificationsAsync();
        return Ok(notifications);
    }

    
}
