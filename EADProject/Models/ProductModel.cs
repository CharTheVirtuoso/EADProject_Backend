using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public int StockQuantity { get; set; }

        // Use ObjectId for referencing the Vendor (UserModel's Id)
        [BsonRepresentation(BsonType.ObjectId)]
        public string VendorId { get; set; }  // Refers to UserModel's Id

        public string Category { get; set; }
        public bool IsLowStock { get; set; } = false;// Flag to check low stock alerts
    }
}
