using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

        public async Task<ProductModel> CreateProductAsync(ProductModel product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task<ProductModel> GetProductByIdAsync(string id)
        {
            return await _products.Find(product => product.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ProductModel>> GetProductsByCategoryAsync(string category)
        {
            return await _products.Find(product => product.Category == category).ToListAsync();
        }

        public async Task UpdateProductAsync(string id, ProductModel updatedProduct)
        {
            await _products.ReplaceOneAsync(product => product.Id == id, updatedProduct);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _products.DeleteOneAsync(product => product.Id == id);
        }

        public async Task ActivateProductAsync(string id)
        {
            var update = Builders<ProductModel>.Update.Set(p => p.IsActive, true).Set(p => p.IsDeactivated, false);
            await _products.UpdateOneAsync(p => p.Id == id, update);
        }

        public async Task DeactivateProductAsync(string id)
        {
            var update = Builders<ProductModel>.Update.Set(p => p.IsActive, false).Set(p => p.IsDeactivated, true);
            await _products.UpdateOneAsync(p => p.Id == id, update);
        }

        public async Task UpdateStockAsync(string id, int newStockQuantity)
        {
            var update = Builders<ProductModel>.Update.Set(p => p.StockQuantity, newStockQuantity);
            await _products.UpdateOneAsync(p => p.Id == id, update);

            // Notify vendor if stock is low
            var product = await GetProductByIdAsync(id);
            if (newStockQuantity < 5) // Assuming 5 is the low stock threshold
            {
                await NotifyVendorLowStock(product.VendorId, product.Name);
            }
        }

        private async Task NotifyVendorLowStock(string vendorId, string productName)
        {
            // Send notification to the vendor
            // Code for sending notification (email, etc.) to vendor about low stock
        }

        public async Task<bool> CheckIfProductInPendingOrder(string productId)
        {
            // Check if the product is part of any pending orders
            // Return true if it is, false otherwise
            return false; // Replace with actual logic
        }
    }
}
