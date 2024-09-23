using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class UserService
    {
        private readonly IMongoCollection<UserModel> _users;

        public UserService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<UserModel>("Users");
        }

        // Create a new user
        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        // Get a user by ID
        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            var objectId = new ObjectId(id); // Convert string ID to ObjectId
            return await _users.Find(user => user.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        // Get all users
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        // Update a user account
        public async Task<bool> UpdateUserAsync(string id, UserModel updatedUser)
        {
            var objectId = new ObjectId(id);
            var result = await _users.ReplaceOneAsync(user => user.Id == objectId.ToString(), updatedUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Deactivate a user account
        public async Task<bool> DeactivateUserAccountAsync(string id)
        {
            var objectId = new ObjectId(id);
            var update = Builders<UserModel>.Update.Set(user => user.IsActive, false);
            var result = await _users.UpdateOneAsync(user => user.Id == objectId.ToString(), update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Reactivate a user account (CSR only)
        public async Task<bool> ReactivateUserAccountAsync(string id)
        {
            var objectId = new ObjectId(id);
            var update = Builders<UserModel>.Update.Set(user => user.IsActive, true);
            var result = await _users.UpdateOneAsync(user => user.Id == objectId.ToString(), update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
