using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace EADProject.Models
{
    public class OrderItemModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // Product associated with this order item.
        public string ProductId { get; set; }

        // Quantity of the product ordered.
        public int Quantity { get; set; }

        // Price at the time of ordering.
        public decimal Price { get; set; }
    }

    public class OrderModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        // User who placed the order.
        public string UserId { get; set; }

        // Collection of order items.
        public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();

        // Status of the order (e.g., "Processing", "Shipped", "Delivered").
        public string Status { get; set; } = "Processing";

        // Shipping address provided by the customer.
        public string ShippingAddress { get; set; }

        // Total cost of the order.
        public decimal TotalAmount { get; set; }

        // Shipping charges.
        public decimal ShippingCharges { get; set; }

        // Date and time when the order was placed.
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Vendor-specific delivery statuses.
        public Dictionary<string, bool> VendorDeliveryStatus { get; set; } = new Dictionary<string, bool>();
    }
}

