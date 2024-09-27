using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // Create a new product (Vendor only)
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductModel product)
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return Ok(createdProduct);
        }

        // Update an existing product (Vendor only)
        [HttpPut("updateProduct/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] ProductModel product)
        {
            var updated = await _productService.UpdateProductAsync(id, product);
            if (!updated)
            {
                return NotFound();
            }
            return Ok("Product updated successfully");
        }

        // Delete a product (Vendor only)
        [HttpDelete("deleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok("Product deleted successfully");
        }

        // Activate/Deactivate a product (Vendor/Admin)
        [HttpPatch("activateProduct/{id}")]
        public async Task<IActionResult> ActivateProduct(string id, [FromQuery] bool isActive)
        {
            var updated = await _productService.UpdateProductActivationAsync(id, isActive);
            if (!updated)
            {
                return NotFound();
            }
            return Ok($"Product {(isActive ? "activated" : "deactivated")} successfully");
        }

        // Update product stock (Vendor only)
        [HttpPatch("stock/{id}")]
        public async Task<IActionResult> UpdateProductStock(string id, [FromQuery] int quantity)
        {
            var updated = await _productService.UpdateProductStockAsync(id, quantity);
            if (!updated)
            {
                return NotFound();
            }
            return Ok("Product stock updated successfully");
        }

        // Get all products (Admin/CSR)
        [HttpGet("getAllProducts")]
        public async Task<ActionResult<List<ProductModel>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // Get products by Vendor ID (Vendor only)
        [HttpGet("getProductsByVendor/{vendorId}")]
        public async Task<ActionResult<List<ProductModel>>> GetProductsByVendor(string vendorId)
        {
            var products = await _productService.GetProductsByVendorAsync(vendorId);
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        // Get products by category
        [HttpGet("getProductsByCategory/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            if (products == null || products.Count == 0)
            {
                return NotFound(new { Message = "No products found in this category." });
            }

            return Ok(products);
        }

        // Get product by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
