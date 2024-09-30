/***************************************************************
 * File Name: CategoryService.cs
 * Description: This service provides data access methods for handling
 *              product categories in the MongoDB database. It allows 
 *              creation, retrieval, and updating of category status.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: This service interacts with the MongoDB collection 'Categories'
 *        to perform CRUD operations and category status updates.
 ***************************************************************/

using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class CategoryService
    {
        private readonly IMongoCollection<CategoryModel> _categories;
        private readonly IMongoCollection<ProductModel> _products;

        // Constructor that initializes MongoDB collection from settings
        public CategoryService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _categories = database.GetCollection<CategoryModel>("Categories");
            _products = database.GetCollection<ProductModel>("Products");
        }

        // Method to create a new category in the database
        public async Task CreateCategoryAsync(CategoryModel newCategory)
        {
            // Inserts the new category into the MongoDB 'Categories' collection
            await _categories.InsertOneAsync(newCategory);
        }

        // Method to retrieve all categories from the database
        public async Task<List<CategoryModel>> GetAllCategoriesAsync()
        {
            // Returns all categories from the 'Categories' collection
            return await _categories.Find(category => true).ToListAsync();
        }

        // Method to retrieve a category by its unique ID
        public async Task<CategoryModel> GetCategoryByIdAsync(string id)
        {
            // Finds and returns the category with the matching ID
            return await _categories.Find(category => category.Id == id).FirstOrDefaultAsync();
        }

        // Method to update the active status of a category
        public async Task UpdateCategoryStatusAsync(string id, bool isActive)
        {
            // Filters by category ID and updates the 'IsActive' status
            var filter = Builders<CategoryModel>.Filter.Eq(c => c.Id, id);
            var update = Builders<CategoryModel>.Update.Set(c => c.IsActive, isActive);
            await _categories.UpdateOneAsync(filter, update);
        }

        // Method to retrieve all categories with their product count
        public async Task<List<CategoryModel>> GetCategoriesWithProductCountAsync()
        {
            var categories = await _categories.Find(category => true).ToListAsync();

            foreach (var category in categories)
            {
                // Query to count the number of products associated with each category
                var productCount = await _products.CountDocumentsAsync(p => p.CategoryName == category.CategoryName);
                category.CategoryCount = (int)productCount;
            }

            return categories;
        }
    }
}
