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

    // Request for order cancellation
    [HttpPost("cancel-order/{OrderId}")]
    public async Task<IActionResult> RequestOrderCancellation(string OrderId)
    {
        // Create the cancellation message
        var message = $"Requested to cancel OrderID {OrderId}";

        // Call the service to handle the cancellation request
        var result = await _customerNotificationService.RequestOrderCancellationAsync(OrderId, message);

        if (result)
        {
            // Return a success response if the cancellation was successful
            return Ok(new { message = "Order cancellation request has been sent.", OrderId = OrderId });
        }
        else
        {
            // Return a failure response if the cancellation request failed
            return BadRequest(new { message = "Failed to request order cancellation.", OrderId = OrderId });
        }
    }

    // Get unread vendor notifications
    [HttpGet("unread")]
    public async Task<IActionResult> GetUnreadCustomerNotifications()
    {
        var notifications = await _customerNotificationService.GetUnreadCustomerNotificationsAsync();
        return Ok(notifications);
    }


}

