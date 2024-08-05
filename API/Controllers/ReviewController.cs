using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedReviews(
            [FromQuery] string? userId,
            [FromQuery] string? productId,
            [FromQuery] int? rating,
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "date_desc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedReviews = await _reviewService.GetPaginatedReviews(searchQuery, sortBy, userId, productId, rating, pageIndex, pageSize);
                return Ok(paginatedReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated reviews: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(string id)
        {
            try
            {
                var review = await _reviewService.GetReviewById(id);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving review by ID: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewModel reviewModel)
        {
            try
            {
                var review = await _reviewService.AddReview(reviewModel);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding review: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewModel reviewModel, [FromQuery] string id)
        {
            try
            {
                var review = await _reviewService.UpdateReview(reviewModel, id);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating review: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewById(string id)
        {
            try
            {
                await _reviewService.DeleteReview(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting review: {ex.Message}");
            }
        }

        [HttpGet("GetAverageRating/{productId}")]
        public async Task<IActionResult> GetAverageRating(string productId)
        {
            try
            {
                var reviews = await _reviewService.GetAverageRating(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving reviews by product ID: {ex.Message}");
            }
        }
    }
}
