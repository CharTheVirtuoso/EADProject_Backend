namespace EADProject.Models
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int StockQuantity { get; set; }
        public string VendorId { get; set; }
    }
}

