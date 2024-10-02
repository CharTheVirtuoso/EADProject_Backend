using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class VendorNotificationService
    {
        private readonly IMongoCollection<NotificationModel> _notifications;

        public VendorNotificationService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _notifications = database.GetCollection<NotificationModel>("Notifications");
        }

        // Create a notification for a vendor
        public async Task CreateVendorNotificationAsync(string message)
        {
            var notification = new NotificationModel
            {
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                Type = NotificationType.Vendor // Notification for vendor
            };

            await _notifications.InsertOneAsync(notification);
        }

        // Get unread notifications for a vendor
        public async Task<List<NotificationModel>> GetUnreadVendorNotificationsAsync()
        {
            return await _notifications.Find(n => n.IsRead == false && n.Type == NotificationType.Vendor).ToListAsync();
        }
    }
}
