using EADProject.Models;
using EADProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EADProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // POST: api/rating
        [HttpPost("AddRating")]
        public async Task<IActionResult> AddRating([FromBody] RatingModel rating)
        {
            await _ratingService.AddRating(rating.VendorId, rating.CustomerId, rating.Rating, rating.Review);
            return Ok(new { message = "Rating added successfully!" });
        }

        // GET: api/rating/vendor/{vendorId}
        [HttpGet("getAverageRating/{vendorId}")]
        public async Task<IActionResult> GetAverageRating(string vendorId)
        {
            var averageRating = await _ratingService.GetAverageRating(vendorId);
            return Ok(new { averageRating });
        }
    }
}

