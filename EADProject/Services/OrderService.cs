using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /**************************************Endpoints needed for the mobile app ***************************************/

        // Create a new order
        public async Task<OrderModel> CreateOrder(OrderModel order)
        {
            await _orders.InsertOneAsync(order);
            return order;
        }


        // Get all orders (Customers/CSR/Admin can list all orders)
        public async Task<List<OrderModel>> GetAllOrders()
        {
            return await _orders.Find(o => true).ToListAsync();
        }

        // Retrieve an order by its ID
        public async Task<OrderModel> GetOrderById(string orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            return order;
        }

        /**************************************Endpoints needed for the web app dashboard***************************************/


        // Get orders by order status
        public async Task<List<OrderModel>> GetOrdersByStatus(OrderStatus status)
        {
            return await _orders.Find(order => order.Status == status).ToListAsync();
        }

        // Get all distinct order statuses with their count
        public async Task<Dictionary<OrderStatus, int>> GetOrderStatusCategoriesWithCountAsync()
        {
            var result = await _orders.Aggregate()
                .Group(order => order.Status, g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Convert the result to a dictionary
            var statusCountDict = result.ToDictionary(r => r.Status, r => r.Count);

            return statusCountDict;
        }


        // Get orders by vendor ID
        public async Task<List<OrderModel>> GetOrdersByVendorId(string vendorId)
        {
            return await _orders.Find(order => order.Items.Any(item => item.VendorId == vendorId)).ToListAsync();
        }

        // Update order status from VendorReady to Delivered
        public async Task<bool> UpdateOrderStatusToDelivered(string orderId)
        {
            var filter = Builders<OrderModel>.Filter.Where(order => order.Id == orderId && order.Status == OrderStatus.VendorReady);
            var update = Builders<OrderModel>.Update.Set(order => order.Status, OrderStatus.Delivered);
            var result = await _orders.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        // Update order status from Processing to Canceled
        public async Task<bool> UpdateOrderStatusToCanceled(string orderId)
        {
            var filter = Builders<OrderModel>.Filter.Where(order => order.Id == orderId && order.Status == OrderStatus.Processing);
            var update = Builders<OrderModel>.Update.Set(order => order.Status, OrderStatus.Canceled);
            var result = await _orders.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        // Update VendorOrderStatus from Pending to ReadyForDelivery
        public async Task<bool> UpdateVendorOrderStatus(string orderId, string vendorId)
        {
            var filter = Builders<OrderModel>.Filter.Where(order => order.Id == orderId && order.VendorDeliveryStatus[vendorId] == VendorOrderStatus.Pending);
            var update = Builders<OrderModel>.Update.Set(order => order.VendorDeliveryStatus[vendorId], VendorOrderStatus.ReadyForDelivery);
            var result = await _orders.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}
