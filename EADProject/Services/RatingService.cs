using EADProject.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace EADProject.Services
{
    public class RatingService
    {
        private readonly IMongoCollection<RatingModel> _ratings;

        public RatingService(IMongoClient mongoClient, IOptions<MongoDBSettings> settings )

        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            // Assuming 'Ratings' is the collection for storing vendor ratings
            _ratings = database.GetCollection<RatingModel>("Ratings");
        }

        // Add a rating for a vendor
        public async Task AddRating(string vendorId, int rating, string? review = null)
        {
            var newRating = new RatingModel
            {
                VendorId = vendorId,
                Rating = rating,
                Review = review
            };

            await _ratings.InsertOneAsync(newRating);
        }

        // Get the average rating for a specific vendor
        public async Task<double> GetAverageRating(string vendorId)
        {
            var vendorRatings = await _ratings.Find(r => r.VendorId == vendorId).ToListAsync();
            if (vendorRatings.Count == 0) return 0; // If no ratings exist, return 0

            double averageRating = vendorRatings.Average(r => r.Rating);
            return averageRating;
        }

        // New method to fetch reviews
        public async Task<List<RatingModel>> GetReviews(string vendorId)
        {
            return await _ratings.Find(r => r.VendorId == vendorId).ToListAsync();
        }
    }
}
