using EADProject.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<ProductModel> _products;

        public ProductService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _products = database.GetCollection<ProductModel>("Products");
        }
        // Create a new product
        public async Task<ProductModel> CreateProductAsync(ProductModel product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        // Update an existing product
        public async Task<bool> UpdateProductAsync(string id, ProductModel updatedProduct)
        {
            var result = await _products.ReplaceOneAsync(p => p.Id == id, updatedProduct);
            return result.ModifiedCount > 0;
        }

        // Delete a product (only if it’s not part of a pending order)
        public async Task<bool> DeleteProductAsync(string id)
        {
            var result = await _products.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        // Activate/Deactivate a product
        public async Task<bool> UpdateProductActivationAsync(string id, bool isActive)
        {
            var update = Builders<ProductModel>.Update.Set(p => p.IsActive, isActive);
            var result = await _products.UpdateOneAsync(p => p.Id == id, update);
            return result.ModifiedCount > 0;
        }

        // Get all products for a vendor
        public async Task<List<ProductModel>> GetProductsByVendorAsync(string vendorId)
        {
            return await _products.Find(p => p.VendorId == vendorId).ToListAsync();
        }

        // Get product by ID
        public async Task<ProductModel> GetProductByIdAsync(string id)
        {
            return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        // Get product by category
        public async Task<List<ProductModel>> GetProductsByCategoryAsync(string category)
        {
            return await _products.Find(product => product.Category == category && product.IsActive).ToListAsync();
        }

        // Update stock for a product and check for low stock
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

            // If low stock, you could trigger an alert/notification to the vendor here.
            if (isLowStock)
            {
                // Trigger alert (implementation depends on your notification system)
            }

            return result.ModifiedCount > 0;
        }

        // Get all products (for Admin and CSR)
        public async Task<List<ProductModel>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }
    }
}
