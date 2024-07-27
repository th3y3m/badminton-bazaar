using Microsoft.AspNetCore.Mvc;
using Services;
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
            var paginatedReviews = await _reviewService.GetPaginatedReviews(searchQuery, sortBy, userId, productId, rating, pageIndex, pageSize);
            return Ok(paginatedReviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(string id)
        {
            var review = await _reviewService.GetReviewById(id);
            return Ok(review);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewModel reviewModel)
        {
            var review = await _reviewService.AddReview(reviewModel);
            return Ok(review);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReview([FromBody] ReviewModel reviewModel, [FromQuery] string id)
        {
            var review = await _reviewService.UpdateReview(reviewModel, id);
            return Ok(review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReviewById(string id)
        {
            await _reviewService.DeleteReview(id);
            return Ok();
        }
    }
}
