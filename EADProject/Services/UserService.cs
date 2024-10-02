/***************************************************************
 * File Name: UserService.cs
 * Description: This class provides methods for managing user accounts, 
 *              including creation, retrieval, update, and deactivation 
 *              using MongoDB as the database. 
 *              It supports asynchronous operations for scalability.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Dependencies: 
 *   - MongoDB.Driver: For MongoDB access
 *   - EADProject.Models.UserModel: Represents the user data structure.
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
        private readonly AdminNotificationService _notificationService;

        // Constructor: Initializes the MongoDB collection for users using client and settings.
        public UserService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings, AdminNotificationService notificationService)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<UserModel>("Users");
            _notificationService = notificationService; // Injecting notification service
        }

        // Method: Sign up a new user for the 'Customer' role with pending approval status.
        public async Task<bool> SignUpAsync(UserModel user)
        {
            // Set role to 'Customer' and status to 'Pending'.
            user.Role = "Customer";
            user.UserStatus = "Pending"; // Pending approval from admin
            user.IsActive = false; // Account not active until admin approval

            // Insert the new user into the database
            await _users.InsertOneAsync(user);

            // Create a notification for the CSR admin.
            var message = $"New user {user.Email} signed up and requires approval.";
            await _notificationService.CreateAdminNotificationAsync(message);

            return true;
        }

        // Method: Log in an existing user by verifying email and password.
        // Only allows login if the account is active and approved.
        public async Task<UserModel?> LoginAsync(string email, string password)
        {
            var user = await _users.Find(x => x.Email == email && x.Password == password).FirstOrDefaultAsync();

            // Return the user only if the account is active
            if (user != null && user.IsActive)
            {
                return user;
            }

            return null;
        }

        // Method: Admin approves a user by changing the user's status to 'Approved' 
        // and activating their account.
        public async Task<bool> ApproveUserAsync(string userId)
        {
            var objectId = ObjectId.Parse(userId);

            // Filter the user by ID and update their status to approved and active
            var filter = Builders<UserModel>.Filter.Eq("_id", objectId);
            var update = Builders<UserModel>.Update
                .Set("UserStatus", "Approved")
                .Set("IsActive", true);

            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Method: Admin rejects a user's registration by updating their status to 'Rejected'.
        public async Task<bool> RejectUserAsync(string userId)
        {
            var objectId = ObjectId.Parse(userId);

            // Filter the user by ID and update their status to 'Rejected'
            var filter = Builders<UserModel>.Filter.Eq("_id", objectId);
            var update = Builders<UserModel>.Update.Set("UserStatus", "Rejected");

            var result = await _users.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        // Method: Create a new user in the system with 'Approved' status and active account.
        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
            // Automatically approve the user and activate their account.
            user.UserStatus = "Approved";
            user.IsActive = true;

            await _users.InsertOneAsync(user);
            return user;
        }

        // Method: Retrieve a user by their unique ID from the database.
        public async Task<UserModel> GetUserByIdAsync(string id)
        {
            var objectId = new ObjectId(id); // Convert string ID to ObjectId
            return await _users.Find(user => user.Id == objectId.ToString()).FirstOrDefaultAsync();
        }

        // Method: Retrieve a list of all users from the database.
        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            // Return all users from the collection.
            return await _users.Find(user => true).ToListAsync();
        }

        // Method: Update an existing user's information.
        public async Task<bool> UpdateUserAsync(string id, UserModel updatedUser)
        {
            var objectId = new ObjectId(id);
            var result = await _users.ReplaceOneAsync(user => user.Id == objectId.ToString(), updatedUser);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Method: Deactivate a user account by setting 'IsActive' to false.
        public async Task<bool> DeactivateUserAccountAsync(string id)
        {
            var objectId = new ObjectId(id);
            var update = Builders<UserModel>.Update.Set(user => user.IsActive, false);
            var result = await _users.UpdateOneAsync(user => user.Id == objectId.ToString(), update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Method: Reactivate a previously deactivated user account by setting 'IsActive' to true.
        public async Task<bool> ReactivateUserAccountAsync(string id)
        {
            var objectId = new ObjectId(id);
            var update = Builders<UserModel>.Update.Set(user => user.IsActive, true);
            var result = await _users.UpdateOneAsync(user => user.Id == objectId.ToString(), update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        // Method: Retrieve a list of users who are awaiting admin approval (status 'Pending').
        public async Task<List<UserModel>> GetPendingUsersAsync()
        {
            // Return all users whose status is 'Pending'
            return await _users.Find(user => user.UserStatus == "Pending").ToListAsync();
        }
    }
}

