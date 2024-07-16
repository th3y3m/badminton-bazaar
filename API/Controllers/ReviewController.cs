using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public ActionResult GetPaginatedReviews(
            [FromQuery] string? userId,
            [FromQuery] string? productId,
            [FromQuery] int? rating,
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "date_desc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedReviews = _reviewService.GetPaginatedReviews(searchQuery, sortBy, userId, productId, rating, pageIndex, pageSize);
            return Ok(paginatedReviews);
        }

        [HttpGet("{id}")]
        public ActionResult GetReviewById(string id)
        {
            var review = _reviewService.GetReviewById(id);
            return Ok(review);
        }

        [HttpPost]
        public ActionResult AddReview([FromBody] ReviewModel reviewModel)
        {
            var review = _reviewService.AddReview(reviewModel);
            return Ok(review);
        }

        [HttpPut]
        public ActionResult UpdateReview([FromBody] ReviewModel reviewModel, [FromQuery] string id)
        {
            var review = _reviewService.UpdateReview(reviewModel, id);
            return Ok(review);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteReviewById(string id)
        {
            _reviewService.DeleteReview(id);
            return Ok();
        }
    }
}
