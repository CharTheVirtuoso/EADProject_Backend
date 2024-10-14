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

        public CustomerNotificationService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _notifications = database.GetCollection<NotificationModel>("Notifications");
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
    }
}
