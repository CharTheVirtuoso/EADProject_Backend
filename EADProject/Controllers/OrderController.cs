using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // POST: api/Order
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel order)
        {
            var createdOrder = await _orderService.CreateOrder(order);
            return Ok(createdOrder);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        // PUT: api/Order/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string id, [FromBody] OrderStatus status)
        {
            var success = await _orderService.UpdateOrderStatus(id, status);
            if (!success) return NotFound();
            return NoContent();
        }

        // PUT: api/Order/{id}/vendor/{vendorId}/status
        [HttpPut("{id}/vendor/{vendorId}/status")]
        public async Task<IActionResult> UpdateVendorDeliveryStatus(string id, string vendorId, [FromBody] VendorOrderStatus status)
        {
            var success = await _orderService.UpdateVendorDeliveryStatus(id, vendorId, status);
            if (!success) return NotFound();
            return NoContent();
        }

        // PUT: api/Order/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(string id, [FromBody] string cancellationNote)
        {
            var success = await _orderService.CancelOrder(id, cancellationNote);
            if (!success) return NotFound();
            return NoContent();
        }

        // GET: api/Order
        [HttpGet("getAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }
    }
}


//using EADProject.Models;
//using EADProject.Services;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;

//namespace EADProject.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class OrderController : ControllerBase
//    {
//        private readonly OrderService _orderService;

//        public OrderController(OrderService orderService)
//        {
//            _orderService = orderService;
//        }

//        // Create a new order.
//        [HttpPost("createOrder")]
//        public async Task<IActionResult> CreateOrder([FromBody] OrderModel order)
//        {
//            var createdOrder = await _orderService.CreateOrderAsync(order);
//            return Ok(createdOrder);
//        }

//        //// Get all orders by user ID.
//        //[HttpGet("getOrdersByUser/{userId}")]
//        //public async Task<IActionResult> GetOrdersByUserId(string userId)
//        //{
//        //    var orders = await _orderService.GetOrdersByUserIdAsync(userId);
//        //    return Ok(orders);
//        //}

//        // Get all orders.
//        [HttpGet("getAllOrders")]
//        public async Task<IActionResult> GetAllOrders()
//        {
//            var orders = await _orderService.GetAllOrdersAsync();
//            return Ok(orders);
//        }

//        // Get orders by vendor ID.
//        [HttpGet("getOrdersByVendor/{vendorId}")]
//        public async Task<IActionResult> GetOrdersByVendorId(string vendorId)
//        {
//            var orders = await _orderService.GetOrdersByVendorIdAsync(vendorId);
//            return Ok(orders);
//        }

//        // Cancel an order.
//        [HttpPut("cancelOrder/{orderId}")]
//        public async Task<IActionResult> CancelOrder(string orderId)
//        {
//            var success = await _orderService.CancelOrderAsync(orderId);
//            if (success)
//            {
//                return Ok("Order cancelled successfully.");
//            }
//            return NotFound("Order not found or already delivered.");
//        }

//        // Mark the order as delivered by the vendor.
//        [HttpPatch("markDelivered/{orderId}/{vendorId}")]
//        public async Task<IActionResult> MarkOrderDelivered(string orderId, string vendorId)
//        {
//            var success = await _orderService.MarkOrderDeliveredAsync(orderId, vendorId);
//            if (success)
//            {
//                var finalized = await _orderService.FinalizeDeliveryAsync(orderId);
//                if (finalized)
//                {
//                    return Ok("Order fully delivered.");
//                }
//                return Ok("Vendor marked order as delivered. Waiting for other vendors.");
//            }
//            return BadRequest("Failed to mark order as delivered.");
//        }

//        // Update the order status.
//        [HttpPatch("updateOrderStatus/{orderId}")]
//        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] string status)
//        {
//            var success = await _orderService.UpdateOrderStatusAsync(orderId, status);
//            if (success)
//            {
//                return Ok("Order status updated successfully.");
//            }
//            return BadRequest("Failed to update order status.");
//        }

//        // In OrderController.cs
//        [HttpDelete("removeItem/{orderId}/{productId}")]
//        public async Task<IActionResult> RemoveOrderItem(string orderId, string productId)
//        {
//            var success = await _orderService.RemoveOrderItemAsync(orderId, productId);
//            if (success)
//            {
//                return Ok("Item removed successfully.");
//            }
//            return NotFound("Order not found or item does not exist.");
//        }

//    }
//}


