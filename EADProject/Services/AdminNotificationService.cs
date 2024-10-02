using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class AdminNotificationService
    {
        private readonly IMongoCollection<NotificationModel> _notifications;

        public AdminNotificationService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _notifications = database.GetCollection<NotificationModel>("Notifications");
        }

        // Create a notification for an admin
        public async Task CreateAdminNotificationAsync(string message)
        {
            var notification = new NotificationModel
            {
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Type = NotificationType.Admin // Notification for admin
            };

            await _notifications.InsertOneAsync(notification);
        }

        // Get unread notifications for admin
        public async Task<List<NotificationModel>> GetUnreadAdminNotificationsAsync()
        {
            return await _notifications.Find(n => n.IsRead == false && n.Type == NotificationType.Admin).ToListAsync();
        }
    }
}
