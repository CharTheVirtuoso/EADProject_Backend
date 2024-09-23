using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; } = true; // By default, the user is active.
    }
}
