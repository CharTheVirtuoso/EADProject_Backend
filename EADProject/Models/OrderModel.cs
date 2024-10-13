using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class OrderItemModel
{
   
    public string ProductName { get; set; }
    public string VendorId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }


    public VendorOrderStatus VendorStatus { get; set; } = VendorOrderStatus.Processing;  // Default status
}

public enum VendorOrderStatus
{
    Processing,
    Ready,
    
}

public enum OrderStatus
{
   
    Processing,
    VendorReady,
    PartiallyDelivered,
    Delivered,
    Canceled
}


public class OrderModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public List<OrderItemModel> Items { get; set; } = new List<OrderItemModel>();

    //public OrderStatus Status { get; set; } = OrderStatus.Processing;
    public OrderStatus Status { get; set; } = OrderStatus.Processing;  // By default, status is pending.

    public string ShippingAddress { get; set; }

    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    //Order count
    public int? OrderCount { get; set; }

    // Track cancellation requests from customers
    public bool IsCancellationRequested { get; set; } = false;

    // Cancellation note (if CSR/Admin cancels it)
    public string? CancellationNote { get; set; }

    public string OrderId { get; set; }

}


