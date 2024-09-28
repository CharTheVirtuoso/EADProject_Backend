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

        //// Create a new product (Vendor only)
        //[HttpPost("createProduct")]
        //public async Task<IActionResult> CreateProduct([FromBody] ProductModel product)
        //{
        //    var createdProduct = await _productService.CreateProductAsync(product);
        //    return Ok(createdProduct);
        //}

        // POST: api/product/createProduct
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


        // GET: api/product/byVendorId/{vendorId}
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

        // GET: api/product/byCategoryName/{categoryName}
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
