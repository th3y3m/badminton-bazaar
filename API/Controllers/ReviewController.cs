using Microsoft.AspNetCore.Mvc;
using Services;

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

    }
}
