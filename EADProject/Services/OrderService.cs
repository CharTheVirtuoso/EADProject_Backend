using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EADProject.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<OrderModel> _orders;

        public OrderService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _orders = database.GetCollection<OrderModel>("Orders");
        }

        public async Task<OrderModel> CreateOrderAsync(OrderModel order)
        {
            await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<OrderModel> GetOrderByIdAsync(string id)
        {
            return await _orders.Find(order => order.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateOrderStatusAsync(string id, string newStatus)
        {
            var update = Builders<OrderModel>.Update.Set(o => o.OrderStatus, newStatus);
            await _orders.UpdateOneAsync(order => order.Id == id, update);
        }

        public async Task<List<OrderModel>> GetAllOrdersAsync()
        {
            return await _orders.Find(_ => true).ToListAsync();
        }
    }
}
