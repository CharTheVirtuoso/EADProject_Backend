using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace EADProject.Models
{
    // Represents a product item in the cart with a reference to the actual product model and quantity
    public class CartItemModel
    {
        // Reference to the actual product entity
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }

        // Quantity of the product in the cart
        public int Quantity { get; set; }

        // Optionally, you can include a Product object for embedding product details in the cart, if needed.
        [BsonIgnore]
        public ProductModel Product { get; set; }
    }

    // Cart Model with several products for a specific user
    public class UserCartModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // User who owns the cart.
        public string UserId { get; set; }

        // Collection of product items with quantities.
        public List<CartItemModel> Products { get; set; } = new List<CartItemModel>();
    }
}


