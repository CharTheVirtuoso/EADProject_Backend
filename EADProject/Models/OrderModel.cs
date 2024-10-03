using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class OrderItemModel
{
    //public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string VendorId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

//public enum OrderStatus
//{
   
//    Processing,
//    ReadyForDelivery,
//    PartiallyDelivered,
//    Delivered,
//    Canceled
//}

public enum VendorOrderStatus
{
    Pending,
    Ready,
    Delivered
}

public class OrderModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();

    public string Status { get; set; } = "Processing";

    public string ShippingAddress { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public Dictionary<string, VendorOrderStatus> VendorDeliveryStatus { get; set; } = new Dictionary<string, VendorOrderStatus>();

    // Track cancellation requests from customers
    public bool IsCancellationRequested { get; set; } = false;

    // Cancellation note (if CSR/Admin cancels it)
    public string? CancellationNote { get; set; }

    // Notifications tracking (you can expand this)
    public bool IsCustomerNotified { get; set; } = false;
    public bool IsVendorNotified { get; set; } = false;
}



//using MongoDB.Bson;
//using MongoDB.Bson.Serialization.Attributes;
//using System;
//using System.Collections.Generic;

//namespace EADProject.Models
//{
//    public class OrderItemModel
//    {

//        // Product associated with this order item.
//        public string ProductId { get; set; }

//        // Product associated with this order item.
//        public string ProductName { get; set; }

//        // Vendor ID associated with the product.
//        public string VendorId { get; set; }

//        // Quantity of the product ordered.
//        public int Quantity { get; set; }

//        // Price at the time of ordering.
//        public decimal Price { get; set; }
//    }

//    public class OrderModel
//    {
//        [BsonId]
//        [BsonRepresentation(BsonType.ObjectId)]
//        public string? Id { get; set; }


//        // Collection of order items.
//        public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();

//        // Status of the order (e.g., "Processing").
//        public string Status { get; set; } = "Processing";

//        // Shipping address provided by the customer.
//        public string ShippingAddress { get; set; }

//        // Total cost of the order.
//        public decimal TotalAmount { get; set; }

//        //Payment Method
//        public string paymentMethod { get; set;}

//        // Date and time when the order was placed.
//        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

//        // Vendor-specific delivery statuses.
//        public Dictionary<string, bool> VendorDeliveryStatus { get; set; } = new Dictionary<string, bool>();
//    }
//}
