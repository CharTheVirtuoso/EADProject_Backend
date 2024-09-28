using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    public class CategoryModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
