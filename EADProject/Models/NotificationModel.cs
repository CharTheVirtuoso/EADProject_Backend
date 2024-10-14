﻿/***************************************************************
 * File Name: NotificationModel.cs
 * Description: Represents the data model for notifications
 *  Date Created: September 15, 2024
 ***************************************************************/
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace EADProject.Models
{
    public enum NotificationType
    {
        Admin,
        Vendor,
        Customer
    }

    public class NotificationModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("IsRead")]
        public bool IsRead { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("NotificationType")]
        public NotificationType Type { get; set; }  // Admin or Vendor
    }
}
