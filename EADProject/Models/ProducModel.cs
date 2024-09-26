namespace EADProject.Models
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } // Determines if the product is active
        public bool IsDeactivated { get; set; } // Determines if the product is deactivated by the vendor
        public int StockQuantity { get; set; }
        public string VendorId { get; set; }
        public string Category { get; set; } // New field for category
        public bool IsLowStock { get; set; } // Flag to check low stock alerts
    }
}

