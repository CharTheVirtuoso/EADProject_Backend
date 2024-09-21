using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            return await _users.Find(user => user.Id == id).FirstOrDefaultAsync();
        }
    }
}
