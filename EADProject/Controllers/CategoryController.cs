/***************************************************************
 * File Name: CategoryController.cs
 * Description: This controller handles HTTP requests related to 
 *              the management of product categories. It provides 
 *              endpoints for creating, fetching, and updating the 
 *              status of categories in the system.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: This controller interacts with the CategoryService to manage 
 *        categories. Endpoints allow for CRUD operations and status 
 *        updates.
 ***************************************************************/

using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EADProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        // Constructor to inject the CategoryService dependency
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: api/category/createCategory
        // Endpoint to create a new category
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryModel newCategory)
        {
            // Validate input: Ensure category name is provided
            if (newCategory == null || string.IsNullOrEmpty(newCategory.CategoryName))
            {
                return BadRequest("Category name is required.");
            }

            // Call service to create category and return the result
            await _categoryService.CreateCategoryAsync(newCategory);
            return CreatedAtAction(nameof(CreateCategory), new { id = newCategory.Id }, newCategory);
        }

        // GET: api/category/getAllCategories
        // Endpoint to fetch all categories
        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            // Fetch all categories from the service
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // PUT: api/category/{id}/activateCategory
        // Endpoint to activate a category by its ID
        [HttpPut("{id}/activateCategory")]
        public async Task<IActionResult> ActivateCategory(string id)
        {
            // Call service to activate the category
            await _categoryService.UpdateCategoryStatusAsync(id, true);
            return Ok();
        }

        // PUT: api/category/{id}/deactivateCategory
        // Endpoint to deactivate a category by its ID
        [HttpPut("{id}/deactivateCategory")]
        public async Task<IActionResult> DeactivateCategory(string id)
        {
            // Call service to deactivate the category
            await _categoryService.UpdateCategoryStatusAsync(id, false);
            return Ok();
        }

        // PUT: api/category/getAllCategoriesWithProductCount
        // Endpoint to retrieve categories with product count
        [HttpGet("getAllCategoriesWithProductCount")]
        public async Task<ActionResult<List<CategoryModel>>> GetCategoriesWithProductCount()
        {
            var categoriesWithProductCount = await _categoryService.GetCategoriesWithProductCountAsync();
            return Ok(categoriesWithProductCount);
        }
    }
}
