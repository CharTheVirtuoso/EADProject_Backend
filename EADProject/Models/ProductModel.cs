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
        public int StockQuantity { get; set; }
        public bool IsLowStock { get; set; } = false;// Flag to check low stock alerts
        public string VendorId { get; set; }


        //// Foreign Key to link to CategoryModel
        //[BsonRepresentation(BsonType.ObjectId)]
        //public string CategoryId { get; set; }

        public string CategoryName { get; set; } // Category name reference

    }
}
