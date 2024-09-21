namespace EADProject.Models
{
    public class OrderModel
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }

        public List<string> ProductIds { get; set; }
        public string OrderStatus { get; set; } // Processing, Delivered, Canceled, etc.
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
