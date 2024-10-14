using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EADProject.Models
{
    // Represents a rating entity for vendors.
    public class RatingModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        // Vendor ID being rated.
        public string VendorId { get; set; }

        // Rating value (1 to 5 stars, for example).
        public int Rating { get; set; }

        // Review or feedback from the customer (optional).
        public string? Review { get; set; }
    }
}
