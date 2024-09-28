/***************************************************************
 * File Name: ProductModel.cs
 * Description: Represents the Product model which defines the schema 
 *              for products stored in the database. It includes fields 
 *              such as name, description, price, stock quantity, and 
 *              vendor information. The model is used in communication 
 *              with MongoDB.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: This class includes a low stock flag to trigger alerts when 
 *        the stock quantity is low.
 ***************************************************************/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    // Represents a product entity in the system.
    public class ProductModel
    {
        // Unique identifier for the product.
        public string Id { get; set; }

        // Name of the product.
        public string Name { get; set; }

        // Detailed description of the product.
        public string Description { get; set; }

        // Price of the product.
        public decimal Price { get; set; }

        // Quantity of the product in stock.
        public int StockQuantity { get; set; }

        // Flag to indicate if the product stock is low.
        public bool IsLowStock { get; set; } = false;

        // Vendor ID associated with the product.
        public string VendorId { get; set; }

        // Category name to which the product belongs.
        public string CategoryName { get; set; }
    }
}
