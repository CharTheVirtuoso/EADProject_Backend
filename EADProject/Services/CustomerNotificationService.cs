/***************************************************************
 * File Name: CustomerNotificationService.cs
 * Description: Represents the data service for notifications for customer
 * Date Created: September 15, 2024
 ***************************************************************/
using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class CustomerNotificationService
    {
        private readonly IMongoCollection<NotificationModel> _notifications;
        private readonly IMongoCollection<OrderModel> _orders;

        public CustomerNotificationService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _notifications = database.GetCollection<NotificationModel>("Notifications");
            _orders = database.GetCollection<OrderModel>("Orders");
        }

        // Create a notification for an admin
        public async Task CreateCustomerNotificationAsync(string message)
        {
            var notification = new NotificationModel
            {
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Type = NotificationType.Customer // Notification for customer
            };

            await _notifications.InsertOneAsync(notification);
        }

        // Get unread notifications for admin
        public async Task<List<NotificationModel>> GetUnreadCustomerNotificationsAsync()
        {
            return await _notifications.Find(n => n.IsRead == false && n.Type == NotificationType.Customer).ToListAsync();
        }

        public async Task<bool> RequestOrderCancellationAsync(string orderId, string message)
        {
            // Fetch the order from the database using the orderId
            var order = await _orders.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();

            if (order == null)
            {
                // If the order does not exist, return false or throw an exception
                return false;
            }

            // Update the order to indicate that cancellation has been requested
            order.IsCancellationRequested = true;

            // Optional: You can add a note or any additional information related to the cancellation
            order.CancellationNote = message;

            // Save the updated order back into the database
            var updateResult = await _orders.ReplaceOneAsync(o => o.OrderId == orderId, order);

            if (!updateResult.IsAcknowledged)
            {
                // If the update operation fails, return false
                return false;
            }

            // Optionally, create a notification for the cancellation request
            var notification = new NotificationModel
            {
                OrderId = orderId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            // Insert the notification into the notifications collection
            await _notifications.InsertOneAsync(notification);

            // Return true if everything was successful
            return true;
        }


    }
}
