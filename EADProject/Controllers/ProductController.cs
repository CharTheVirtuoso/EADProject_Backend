/***************************************************************
 * File Name: ProductController.cs
 * Description: Defines the API endpoints for managing product-related 
 *              operations. This controller handles product creation, 
 *              updating, deleting, retrieving, and stock management.
 *              The API is accessible to vendors, admins, and CSR.
 * Date Created: September 15, 2024
 * Notes: This controller is part of the API layer that interacts 
 *        with the ProductService class and MongoDB collections.
 ***************************************************************/

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

        // Constructor: Injects ProductService dependency.
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // POST: api/product/createProduct
        // Create a new product. Validates the product category before adding it.
        [HttpPost("createProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductModel product)
        {
            try
            {
                var createdProduct = await _productService.CreateProductAsync(product);
                return Ok(createdProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Update an existing product by its ID (Vendor only).
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

        // Delete a product by its ID (Vendor only).
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

        // Update product stock by ID and quantity (Vendor only).
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

        // Get a list of all products (Admin/CSR).
        [HttpGet("getAllProducts")]
        public async Task<ActionResult<List<ProductModel>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET api/product/name/{name}
        [HttpGet("getProductByName/{name}")]
        public async Task<ActionResult<ProductModel>> GetProductByName(string name)
        {
            var product = await _productService.GetProductsByNameAsync(name);

            if (product == null)
            {
                return NotFound($"No product found with name '{name}'.");
            }

            return Ok(product);
        }

        // GET: api/product/getProductsByVendor/{vendorId}
        // Fetch products by VendorId. Returns a list of products from a specific vendor.
        [HttpGet("getProductsByVendor/{vendorId}")]
        public async Task<IActionResult> GetProductsByVendorId(string vendorId)
        {
            try
            {
                var products = await _productService.GetProductsByVendorIdAsync(vendorId);
                if (products == null || products.Count == 0)
                {
                    return NotFound(new { message = "No products found for the given vendor." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get a product by its unique ID. Returns the product if found.
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

        // GET: api/product/getProductByCategory/{categoryName}
        // Fetch products by category name. Returns a list of products under the specified category.
        [HttpGet("getProductByCategory/{categoryName}")]
        public async Task<IActionResult> GetProductsByCategoryName(string categoryName)
        {
            try
            {
                var products = await _productService.GetProductsByCategoryNameAsync(categoryName);
                if (products == null || products.Count == 0)
                {
                    return NotFound(new { message = "No products found for the given category." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
