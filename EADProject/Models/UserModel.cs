/***************************************************************
 * File Name: UserModel.cs
 * Description: This class represents the structure of the user model
 *              stored in the MongoDB database. It includes fields for 
 *              user identification, login credentials, role, and account status.
 * Author: Chanukya Serasinghe, Nashali Perera
 * Date Created: September 15, 2024
 * Notes: The 'Id' field is represented as a MongoDB ObjectId. The 
 *        IsActive field is true by default, meaning a user account 
 *        is active upon creation unless otherwise specified.
 ***************************************************************/

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    public class UserModel
    {
        // Represents the unique ID of the user, stored as ObjectId in MongoDB.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        //Stores the user's name.
        public string? Name { get; set; }

        // Stores the user's email address.
        public string Email { get; set; }

        // Stores the user's password (ensure it is hashed in real implementations).
        public string Password { get; set; }

        // Stores the user's  address.
        public string? Address { get; set; }

        // Stores the user's role, e.g., Admin, User, CSR.
        public string? Role { get; set; } 

        // Indicates the user status 
        public String UserStatus { get; set; } = "Pending"; // By default, status is pending.

        // Indicates whether the user's account is active or not. Defaults to true.
        public bool IsActive { get; set; } = false; // By default, the user is not active.
    }
}

