using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EADProject.Services
{
    public class CategoryService
    {
        private readonly IMongoCollection<CategoryModel> _categories;

        public CategoryService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _categories = database.GetCollection<CategoryModel>("Categories");
        }


        // Method to create a new category
        public async Task CreateCategoryAsync(CategoryModel newCategory)
        {
            await _categories.InsertOneAsync(newCategory);
        }

        public async Task<List<CategoryModel>> GetAllCategoriesAsync()
        {
            return await _categories.Find(category => true).ToListAsync();
        }

        public async Task<CategoryModel> GetCategoryByIdAsync(string id)
        {
            return await _categories.Find(category => category.Id == id).FirstOrDefaultAsync();
        }

        public async Task UpdateCategoryStatusAsync(string id, bool isActive)
        {
            var filter = Builders<CategoryModel>.Filter.Eq(c => c.Id, id);
            var update = Builders<CategoryModel>.Update.Set(c => c.IsActive, isActive);
            await _categories.UpdateOneAsync(filter, update);
        }
    }
}
