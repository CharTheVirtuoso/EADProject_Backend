using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace EADProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST: api/category
        // Endpoint to create a new category
        [HttpPost("createCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryModel newCategory)
        {
            if (newCategory == null || string.IsNullOrEmpty(newCategory.CategoryName))
            {
                return BadRequest("Category name is required.");
            }

            await _categoryService.CreateCategoryAsync(newCategory);
            return CreatedAtAction(nameof(CreateCategory), new { id = newCategory.Id }, newCategory);
        }

        [HttpGet("getAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPut("{id}/activateCategory")]
        public async Task<IActionResult> ActivateCategory(string id)
        {
            await _categoryService.UpdateCategoryStatusAsync(id, true);
            return Ok();
        }

        [HttpPut("{id}/deactivateCategory")]
        public async Task<IActionResult> DeactivateCategory(string id)
        {
            await _categoryService.UpdateCategoryStatusAsync(id, false);
            return Ok();
        }
    }
}
