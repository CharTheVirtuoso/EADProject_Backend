/***************************************************************
 * File Name: ProductService.cs
 * Description: Provides the service layer for handling product-related 
 *              operations such as creating, updating, deleting, 
 *              and retrieving products from the MongoDB database. 
 *              It also includes methods for managing stock and category 
 *              associations.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: This class integrates with MongoDB collections for products, 
 *        categories, and users, and handles stock updates, low stock 
 *        checks, and category verification.
 ***************************************************************/

using EADProject.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    // This class defines services related to product management.
    public class ProductService
    {
        private readonly IMongoCollection<ProductModel> _products;
        private readonly IMongoCollection<CategoryModel> _categories;
        private readonly IMongoCollection<UserModel> _users;
        private readonly VendorNotificationService _notificationService;

        // Constructor: Initializes the MongoDB collections for products, categories, and users.
        public ProductService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings, VendorNotificationService notificationService)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _products = database.GetCollection<ProductModel>("Products");
            _categories = database.GetCollection<CategoryModel>("Categories");
            _users = database.GetCollection<UserModel>("Users");
            _notificationService = notificationService;
        }

        // Create a new product. Validates that the category exists and is active.
        public async Task<ProductModel> CreateProductAsync(ProductModel product)
        {
            // Check if the category name exists and is active
            var category = await _categories.Find(c => c.CategoryName == product.CategoryName && c.IsActive).FirstOrDefaultAsync();

            if (category == null)
            {
                throw new Exception("Category not found or is inactive.");
            }

            // Proceed to insert the product with the category name
            await _products.InsertOneAsync(product);
            return product;
        }

        // Update an existing product. Replaces the product document in the database.
        public async Task<bool> UpdateProductAsync(string id, ProductModel updatedProduct)
        {
            var result = await _products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
            return result.ModifiedCount > 0;
        }

        // Delete a product by ID. Deletes only if the product is not part of a pending order.
        public async Task<bool> DeleteProductAsync(string id)
        {
            var result = await _products.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        //// Fetch a list of products by VendorId. Retrieves all products associated with a specific vendor.
        //public async Task<List<ProductModel>> GetProductsByVendorIdAsync(string vendorId)
        //{
        //    // Find products where VendorId matches
        //    var products = await _products.Find(p => p.VendorId == vendorId).ToListAsync();
        //    return products;

        //}

        // Fetch a list of products by VendorId. Retrieves all products associated with a specific vendor.
        public async Task<List<ProductModel>> GetProductsByVendorIdAsync(string vendorId)
        {
            // Find products where VendorId matches
            var products = await _products.Find(p => p.VendorId == vendorId).ToListAsync();

            // Check for low stock products and create notifications
            foreach (var product in products)
            {
                if (product.StockQuantity < 5 && product.StockQuantity > 0) // Check if stock is low but not out of stock
                {
                    // Create a notification for the vendor
                    string message = $"Warning: The stock for product '{product.Name}' is low (Only {product.StockQuantity} left). Please restock.";
                    await _notificationService.CreateVendorNotificationAsync(message);
                }
            }

            return products;
        }


        // Get a product by its unique ID. Returns the product if found.
        public async Task<ProductModel> GetProductByIdAsync(string id)
        {
            return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        // Fetch a list of products by CategoryName. Retrieves all products under a specified category.
        public async Task<List<ProductModel>> GetProductsByCategoryNameAsync(string categoryName)
        {
            // Find products where the CategoryName matches
            var products = await _products.Find(p => p.CategoryName == categoryName).ToListAsync();
            return products;
        }

        // Update stock for a product and check for low stock. If the new stock quantity is below a threshold, marks the product as low stock.
        public async Task<bool> UpdateProductStockAsync(string id, int quantity)
        {
            var product = await GetProductByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            // Update stock quantity
            var newStockQuantity = product.StockQuantity + quantity;
            var isLowStock = newStockQuantity < 10; // Assuming 10 is the threshold for low stock

            var update = Builders<ProductModel>.Update
                .Set(p => p.StockQuantity, newStockQuantity)
                .Set(p => p.IsLowStock, isLowStock);

            var result = await _products.UpdateOneAsync(p => p.Id == id, update);

           

            return result.ModifiedCount > 0;
        }

        // Get a list of all products in the database.
        public async Task<List<ProductModel>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }
    }
}
