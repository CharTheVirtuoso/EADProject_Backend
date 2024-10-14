/***************************************************************
 * File Name: CustomerNotificationController.cs
 * Description: Represents the data service for notifications for Customer
 * Date Created: September 15, 2024
 ***************************************************************/
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/customer-notifications")]
[ApiController]
public class CustomerNotificationController : ControllerBase
{
    private readonly CustomerNotificationService _customerNotificationService;

    public CustomerNotificationController(CustomerNotificationService customerNotificationService)
    {
        _customerNotificationService = customerNotificationService;
    }

    // Get unread vendor notifications
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadCustomerNotifications()
    {
        var notifications = await _customerNotificationService.GetUnreadCustomerNotificationsAsync();
        return Ok(notifications);
    }


}

