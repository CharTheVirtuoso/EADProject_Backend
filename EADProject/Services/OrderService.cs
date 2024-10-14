/******************************************************************
 * File Name: OrderService.cs
 * Description: This service class provides methods to interact with 
 *              the Order collection in the MongoDB database. It handles 
 *              creation, retrieval, and updates to order documents, 
 *              supporting functionality for both the mobile app and 
 *              web app dashboard. It also manages order statuses based 
 *              on vendor readiness and customer actions.
 * Date Created: September 15, 2024
 * Notes: This service includes methods that facilitate both customer 
 *        and vendor interactions with orders, allowing for order 
 *        creation, status management, and retrieval by vendor or status.
 ******************************************************************/
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
        private readonly IMongoCollection<ProductModel> _products;

        public OrderService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _orders = database.GetCollection<OrderModel>("Orders");
            _products = database.GetCollection<ProductModel>("Products");
        }

        /**************************************Endpoints needed for the mobile app ***************************************/

        //// Create a new order
        //public async Task<OrderModel> CreateOrder(OrderModel order)
        //{
        //    await _orders.InsertOneAsync(order);
        //    return order;
        //}

        // Create a new order
        public async Task<OrderModel> CreateOrder(OrderModel order)
        {
            // Loop through each item in the order
            foreach (var orderItem in order.Items)
            {
                // Fetch the corresponding product using VendorId and ProductName (or ideally by a unique ProductId if available)
                var product = await _products.Find(p => p.VendorId == orderItem.VendorId && p.Name == orderItem.ProductName).FirstOrDefaultAsync();

                if (product == null)
                {
                    // Handle case where product is not found (e.g., log error, return a failure response, etc.)
                    throw new Exception($"Product '{orderItem.ProductName}' from Vendor '{orderItem.VendorId}' not found.");
                }

                // Check if there is enough stock
                if (product.StockQuantity < orderItem.Quantity)
                {
                    // Handle the case where there is insufficient stock
                    throw new Exception($"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {orderItem.Quantity}");
                }

                // Reduce the stock quantity of the product
                product.StockQuantity -= orderItem.Quantity;

                // If stock quantity is now below a certain threshold, mark the product as low stock
                product.IsLowStock = product.StockQuantity <= 5; // Example threshold for low stock

                // Update the product in the database
                var updateResult = await _products.ReplaceOneAsync(p => p.Id == product.Id, product);

                if (!updateResult.IsAcknowledged)
                {
                    // Handle failure in updating the product
                    throw new Exception($"Failed to update stock for product '{product.Name}'");
                }
            }

            // After all stock updates are successful, insert the order
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

        // Check if all vendor items are ready, and if so, update the order status
        public async Task UpdateOrderStatusBasedOnVendorReadiness(string orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null) return;

            // Check if all items from different vendors are marked as Ready
            bool allVendorsReady = order.Items.All(item => item.VendorStatus == VendorOrderStatus.Ready);

            // Update the order status accordingly
            var newStatus = allVendorsReady ? OrderStatus.VendorReady : OrderStatus.PartiallyDelivered;
            var update = Builders<OrderModel>.Update.Set(order => order.Status, newStatus);
            await _orders.UpdateOneAsync(o => o.Id == orderId, update);
        }

        public async Task<bool> UpdateOrder(OrderModel order)
        {
            var result = await _orders.ReplaceOneAsync(o => o.Id == order.Id, order);
            return result.ModifiedCount > 0;
        }

        // Update the cancellation note for a specific order
        public async Task<bool> UpdateCancellationNoteAsync(string orderId, string cancellationNote)
        {
            var filter = Builders<OrderModel>.Filter.Eq(o => o.Id, orderId);
            var update = Builders<OrderModel>.Update
                            .Set(o => o.CancellationNote, cancellationNote)
                            .Set(o => o.Status, OrderStatus.Canceled);  // Optionally, set the status to "Canceled"

            var result = await _orders.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }


    }
}
