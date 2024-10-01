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
        public async Task<IActionResult> AddProductToCart(string userId, string productId)
        {
            var updatedCart = await _cartService.AddProductToCart(userId, productId);
            return Ok(updatedCart);
        }

        // Get the cart for the user
        [HttpGet("get")]
        public async Task<IActionResult> GetUserCart(string userId)
        {
            var userCart = await _cartService.GetUserCart(userId);
            if (userCart == null)
            {
                return NotFound("Cart not found");
            }
            return Ok(userCart);
        }

        // Remove a product from the cart
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveProductFromCart(string userId, string productId)
        {
            await _cartService.RemoveProductFromCart(userId, productId);
            return NoContent();
        }
    }
}

