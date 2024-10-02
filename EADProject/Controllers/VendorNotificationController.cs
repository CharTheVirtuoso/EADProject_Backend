using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/vendor-notifications")]
[ApiController]
public class VendorNotificationController : ControllerBase
{
    private readonly VendorNotificationService _vendorNotificationService;

    public VendorNotificationController(VendorNotificationService vendorNotificationService)
    {
        _vendorNotificationService = vendorNotificationService;
    }

    // Get unread vendor notifications
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadVendorNotifications()
    {
        var notifications = await _vendorNotificationService.GetUnreadVendorNotificationsAsync();
        return Ok(notifications);
    }

    
}

