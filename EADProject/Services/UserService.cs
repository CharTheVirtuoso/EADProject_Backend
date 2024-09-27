/***************************************************************
 * File Name: UserService.cs
 * Description: This class provides methods for managing user accounts, 
 *              including creation, retrieval, update, and deactivation 
 *              using MongoDB as the database. 
 *              It supports asynchronous operations for scalability.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: This service depends on MongoDB.Driver and a UserModel class. 
 ***************************************************************/

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

        // Constructor: Initializes the MongoDB collection for users.
        public UserService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<UserModel>("Users");
        }

        // User Sign-up for customer role
        public async Task<bool> SignUpAsync(UserModel user)
        {
            // Ensure the role is set to 'Customer'
            user.Role = "Customer";
            user.UserStatus = "Pending"; // Pending approval from admin
            user.IsActive = false; // Account not active until admin approval

            // Insert new user into the database
            await _users.InsertOneAsync(user);
            return true;
        }

        // User login
        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            var user = await _users.Find(x => x.Email == email && x.Password == password).FirstOrDefaultAsync();

            // Check if the user is active and approved
            if (user != null && user.IsActive)
            {
                return user;
            }

            return null;
        }

        // Admin approves user registration
       public async Task<bool> ApproveUserAsync(string userId)
    {
        // Ensure the userId is converted to ObjectId before querying
        var objectId = ObjectId.Parse(userId);

        var filter = Builders<UserModel>.Filter.Eq("_id", objectId);

        // Update the status to "Approved" and set the user as active
        var update = Builders<UserModel>.Update
            .Set("UserStatus", "Approved")
            .Set("IsActive", true);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }


    // Admin  rejects user registration
    public async Task<bool> RejectUserAsync(string userId)
        {

            // Ensure the userId is converted to ObjectId before querying
            var objectId = ObjectId.Parse(userId);

            var filter = Builders<UserModel>.Filter.Eq("_id", objectId);

            // Update the status to "rejected" and set the user as active
            var update = Builders<UserModel>.Update
                .Set("UserStatus", "Rejected");

            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Create a new user in the database.
        // Parameters: UserModel - the user object to be added.
        // Returns: The created UserModel object.
        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
          
            user.UserStatus = "Approved"; // approval from admin
            user.IsActive = true; 

            await _users.InsertOneAsync(user);
            return user;
        }

        // Retrieve a user from the database by their ID.
        // Parameters: id - the user's ID in string format.
        // Returns: The corresponding UserModel if found, or null if not.
        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            var objectId = new ObjectId(id); // Convert string ID to ObjectId
            return await _users.Find(user => user.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        // Retrieve all users from the database.
        // Returns: A list of UserModel objects.
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        // Update an existing user in the database.
        // Parameters: id - the user's ID, updatedUser - the updated user details.
        // Returns: True if the update was successful, false otherwise.
        public async Task<bool> UpdateUserAsync(string id, UserModel updatedUser)
        {
            var objectId = new ObjectId(id);
            var result = await _users.ReplaceOneAsync(user => user.Id == objectId.ToString(), updatedUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Deactivate a user account by setting the 'IsActive' field to false.
        // Parameters: id - the user's ID.
        // Returns: True if the operation was successful, false otherwise.
        public async Task<bool> DeactivateUserAccountAsync(string id)
        {
            var objectId = new ObjectId(id);
            var update = Builders<UserModel>.Update.Set(user => user.IsActive, false);
            var result = await _users.UpdateOneAsync(user => user.Id == objectId.ToString(), update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Reactivate a user account by setting the 'IsActive' field to true (CSR only).
        // Parameters: id - the user's ID.
        // Returns: True if the operation was successful, false otherwise.
        public async Task<bool> ReactivateUserAccountAsync(string id)
        {
            var objectId = new ObjectId(id);
            var update = Builders<UserModel>.Update.Set(user => user.IsActive, true);
            var result = await _users.UpdateOneAsync(user => user.Id == objectId.ToString(), update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }


        // Get all pending users for admin review
        public async Task<List<UserModel>> GetPendingUsersAsync()
        {
            return await _users.Find(user => user.UserStatus == "Pending").ToListAsync();
        }
    
    }
}
