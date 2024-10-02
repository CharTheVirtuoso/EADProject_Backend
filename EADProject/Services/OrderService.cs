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

        // Create a new order
        public async Task<OrderModel> CreateOrder(OrderModel order)
        {
            await _orders.InsertOneAsync(order);
            return order;
        }

        // Retrieve an order by its ID
        public async Task<OrderModel> GetOrderById(string orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            return order;
        }

        // Update the order status (CSR/Admin)
        public async Task<bool> UpdateOrderStatus(string orderId, OrderStatus newStatus)
        {
            var update = Builders<OrderModel>.Update.Set(o => o.Status, newStatus);
            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
            return result.ModifiedCount > 0;
        }

        // Update the delivery status for a specific vendor
        public async Task<bool> UpdateVendorDeliveryStatus(string orderId, string vendorId, VendorOrderStatus newStatus)
        {
            var update = Builders<OrderModel>.Update.Set(o => o.VendorDeliveryStatus[vendorId], newStatus);
            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
            return result.ModifiedCount > 0;
        }

        // Cancel an order (CSR/Admin)
        public async Task<bool> CancelOrder(string orderId, string cancellationNote)
        {
            var update = Builders<OrderModel>.Update
                .Set(o => o.Status, OrderStatus.Canceled)
                .Set(o => o.CancellationNote, cancellationNote);
            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
            return result.ModifiedCount > 0;
        }

        // Get all orders (CSR/Admin can list all orders)
        public async Task<List<OrderModel>> GetAllOrders()
        {
            return await _orders.Find(o => true).ToListAsync();
        }
    }
}








//        // Create a new order.
//        public async Task<OrderModel> CreateOrderAsync(OrderModel order)
//        {
//            await _orders.InsertOneAsync(order);
//            return order;
//        }

//        //// Get all orders for a specific user.
//        //public async Task<List<OrderModel>> GetOrdersByUserIdAsync(string userId)
//        //{
//        //    return await _orders.Find(order => order.UserId == userId).ToListAsync();
//        //}

//        // Get all orders.
//        public async Task<List<OrderModel>> GetAllOrdersAsync()
//        {
//            return await _orders.Find(_ => true).ToListAsync();
//        }

//        // Get orders by vendor ID.
//        public async Task<List<OrderModel>> GetOrdersByVendorIdAsync(string vendorId)
//        {
//            return await _orders.Find(order => order.VendorDeliveryStatus.ContainsKey(vendorId)).ToListAsync();
//        }

//        // Update the order status.
//        public async Task<bool> UpdateOrderStatusAsync(string orderId, string status)
//        {
//            var update = Builders<OrderModel>.Update.Set(o => o.Status, status);
//            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
//            return result.ModifiedCount > 0;
//        }

//        // Cancel the order by updating its status and notifying the customer.
//        public async Task<bool> CancelOrderAsync(string orderId)
//        {
//            var update = Builders<OrderModel>.Update.Set(o => o.Status, "Cancelled");
//            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
//            return result.ModifiedCount > 0;
//        }

//        // Mark the order as delivered by the vendor.
//        public async Task<bool> MarkOrderDeliveredAsync(string orderId, string vendorId)
//        {
//            var update = Builders<OrderModel>.Update.Set($"VendorDeliveryStatus.{vendorId}", true);
//            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
//            return result.ModifiedCount > 0;
//        }

//        // Check if all vendors have delivered and mark the order as fully delivered.
//        public async Task<bool> FinalizeDeliveryAsync(string orderId)
//        {
//            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
//            if (order != null)
//            {
//                var allDelivered = true;
//                foreach (var status in order.VendorDeliveryStatus.Values)
//                {
//                    if (!status)
//                    {
//                        allDelivered = false;
//                        break;
//                    }
//                }

//                if (allDelivered)
//                {
//                    var update = Builders<OrderModel>.Update.Set(o => o.Status, "Delivered");
//                    var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
//                    return result.ModifiedCount > 0;
//                }
//            }
//            return false;
//        }

//        // In OrderService.cs
//        public async Task<bool> RemoveOrderItemAsync(string orderId, string productId)
//        {
//            var update = Builders<OrderModel>.Update.PullFilter(
//                o => o.Items,
//                item => item.ProductId == productId);

//            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);
//            return result.ModifiedCount > 0;
//        }

//    }
//}