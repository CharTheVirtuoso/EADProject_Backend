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

        public async Task UpdateProductAsync(string id, ProductModel updatedProduct)
        {
            await _products.ReplaceOneAsync(product => product.Id == id, updatedProduct);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _products.DeleteOneAsync(product => product.Id == id);
        }

        public async Task<List<ProductModel>> GetAllProductsAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task UpdateStockAsync(string id, int newStockQuantity)
        {
            var update = Builders<ProductModel>.Update.Set(p => p.StockQuantity, newStockQuantity);
            await _products.UpdateOneAsync(p => p.Id == id, update);
        }
    }
}
