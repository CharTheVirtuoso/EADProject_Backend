using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EADProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }


        // Add a product to the cart
        [HttpPost("addToCart")]
        public async Task<IActionResult> AddProductToCart([FromBody] CartItemRequest request)
        {
            var updatedCart = await _cartService.AddProductToCart(request.UserId, request.ProductId, request.Quantity);
            return Ok(updatedCart);
        }

        // Get the cart for the user
        [HttpGet("getCart")]
        public async Task<IActionResult> GetUserCart(string userId)
        {
            var userCart = await _cartService.GetUserCart(userId);
            if (userCart == null)
            {
                return NotFound("Cart not found");
            }
            return Ok(userCart);
        }

        // Decrement a product quantity in the cart
        [HttpPost("decrement")]
        public async Task<IActionResult> DecrementProductQuantity([FromBody] DecrementRequest request)
        {
            var updatedCart = await _cartService.DecrementProductQuantity(request.UserId, request.ProductId);
            if (updatedCart == null)
            {
                return NotFound("Cart not found");
            }
            return Ok(updatedCart);
        }


        //Remove a product from the cart
        [HttpDelete("removeFromCart")]
        public async Task<IActionResult> RemoveProductFromCart([FromBody] RemoveFromCartRequest request)
        {
          await _cartService.RemoveProductFromCart(request.UserId, request.ProductId);
          return NoContent();
        }
    }
}

