using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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

        /**************************************Endpoints needed for the mobile app ***************************************/

        // POST: api/Order
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel order)
        {
            var createdOrder = await _orderService.CreateOrder(order);
            return Ok(createdOrder);
        }

        // GET: api/Order
        [HttpGet("getAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrders();
            return Ok(orders);
        }

        // GET: api/Order/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(string id)
        {
            var order = await _orderService.GetOrderById(id);
            if (order == null) return NotFound();
            return Ok(order);
        }




        /**************************************Endpoints needed for the web app dashboard***************************************/

        // GET: api/order/status/{status}
        [HttpGet("getOrdersByStatus/{status}")]
        public async Task<ActionResult<List<OrderModel>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatus(status);
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found with the specified status.");
            }

            return Ok(orders);
        }

        // GET: api/order/statusCategoriesWithCount
        [HttpGet("statusCategoriesWithCount")]
        public async Task<ActionResult<Dictionary<OrderStatus, int>>> GetOrderStatusCategoriesWithCount()
        {
            var statusCountDict = await _orderService.GetOrderStatusCategoriesWithCountAsync();
            return Ok(statusCountDict);
        }


        // GET: api/order/vendor/{vendorId}
        [HttpGet("getOrdersByVendorId/{vendorId}")]
        public async Task<ActionResult<List<OrderModel>>> GetOrdersByVendorId(string vendorId)
        {
            var orders = await _orderService.GetOrdersByVendorId(vendorId);
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the specified vendor.");
            }

            return Ok(orders);
        }

        // PUT: api/order/{orderId}/status/delivered
        [HttpPut("{orderId}/updateStatus/delivered")]
        public async Task<IActionResult> UpdateOrderStatusToDelivered(string orderId)
        {
            var result = await _orderService.UpdateOrderStatusToDelivered(orderId);
            if (!result)
            {
                return BadRequest("Order status could not be updated to Delivered.");
            }

            return NoContent();
        }

        // PUT: api/order/{orderId}/status/canceled
        [HttpPut("{orderId}/updateStatus/canceled")]
        public async Task<IActionResult> UpdateOrderStatusToCanceled(string orderId)
        {
            var result = await _orderService.UpdateOrderStatusToCanceled(orderId);
            if (!result)
            {
                return BadRequest("Order status could not be updated to Canceled.");
            }

            return NoContent();
        }

        [HttpPut("{orderId}/vendor/{vendorId}/updateVendorStatus/ready")]
        public async Task<IActionResult> UpdateVendorStatusToReady(string orderId, string vendorId)
        {
            // Retrieve the order by ID
            var order = await _orderService.GetOrderById(orderId);
            if (order == null) return NotFound();

            // Find the vendor's items and update their status to Ready
            foreach (var item in order.Items.Where(i => i.VendorId == vendorId))
            {
                item.VendorStatus = VendorOrderStatus.Ready;
            }

            // Instead of inserting a new order, update the existing order
            var updateResult = await _orderService.UpdateOrder(order);

            if (!updateResult)
            {
                return BadRequest("Failed to update the order.");
            }

            // Check if all vendors are ready and update the overall order status
            await _orderService.UpdateOrderStatusBasedOnVendorReadiness(orderId);

            return NoContent();
        }




    }
}