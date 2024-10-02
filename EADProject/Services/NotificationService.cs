using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<NotificationModel> _notifications;

        public NotificationService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _notifications = database.GetCollection<NotificationModel>("Notifications");
        }

        // Create a new notification
        public async Task CreateNotificationAsync(string message)
        {
            var notification = new NotificationModel
            {
                Message = message,
                IsRead = false, // Unread by default
                CreatedAt = DateTime.UtcNow
            };

            await _notifications.InsertOneAsync(notification);
        }

        // Get unread notifications
        public async Task<List<NotificationModel>> GetUnreadNotificationsAsync()
        {
            return await _notifications.Find(n => n.IsRead == false).ToListAsync();
        }

        // Mark notification as read
        public async Task MarkNotificationAsReadAsync(string id)
        {
            var filter = Builders<NotificationModel>.Filter.Eq("_id", ObjectId.Parse(id));
            var update = Builders<NotificationModel>.Update.Set(n => n.IsRead, true);

            await _notifications.UpdateOneAsync(filter, update);
        }
    }
}
