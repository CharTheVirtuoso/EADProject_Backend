using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace EADProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductModel product)
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductModel updatedProduct)
        {
            await _productService.UpdateProductAsync(id, updatedProduct);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var isPendingOrder = await _productService.CheckIfProductInPendingOrder(id);
            if (isPendingOrder)
            {
                return BadRequest("Cannot delete a product that is part of a pending order.");
            }
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/activate")]
        public async Task<IActionResult> ActivateProduct(string id)
        {
            await _productService.ActivateProductAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/deactivate")]
        public async Task<IActionResult> DeactivateProduct(string id)
        {
            await _productService.DeactivateProductAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/updatestock")]
        public async Task<IActionResult> UpdateStock(string id, [FromBody] int stockQuantity)
        {
            await _productService.UpdateStockAsync(id, stockQuantity);
            return NoContent();
        }
    }
}