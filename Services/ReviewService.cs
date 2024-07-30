using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.Helper;
using Services.Interface;
using Services.Models;

namespace Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<PaginatedList<Review>> GetPaginatedReviews(
            string searchQuery,
            string sortBy,
            string userId,
            string productId,
            int? rating,
            int pageIndex,
            int pageSize)
        {
            try
            {
                var dbSet = await _reviewRepository.GetDbSet();
                var source = dbSet.AsNoTracking();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    source = source.Where(p => p.ReviewText.ToLower().Contains(searchQuery.ToLower()));
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    source = source.Where(p => p.UserId == userId);
                }

                if (!string.IsNullOrEmpty(productId))
                {
                    source = source.Where(p => p.ProductId == productId);
                }

                if (rating.HasValue)
                {
                    source = source.Where(p => p.Rating == rating);
                }

                // Apply sorting
                source = sortBy switch
                {
                    "rating_asc" => source.OrderBy(p => p.Rating),
                    "rating_desc" => source.OrderByDescending(p => p.Rating),
                    "reviewdate_asc" => source.OrderBy(p => p.ReviewDate),
                    "reviewdate_desc" => source.OrderByDescending(p => p.ReviewDate),
                    _ => source
                };

                // Apply pagination
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                return new PaginatedList<Review>(items, count, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving paginated reviews: {ex.Message}");
            }
        }

        public async Task<Review> GetReviewById(string id)
        {
            try
            {
                return await _reviewRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving review by ID: {ex.Message}");
            }
        }

        public async Task<Review> AddReview(ReviewModel reviewModel)
        {
            try
            {
                var review = new Review
                {
                    ReviewId = "R" + GenerateId.GenerateRandomId(5),
                    UserId = reviewModel.UserId,
                    ProductId = reviewModel.ProductId,
                    Rating = reviewModel.Rating,
                    ReviewText = reviewModel.ReviewText,
                    ReviewDate = DateTime.Now
                };

                await _reviewRepository.Add(review);

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding review: {ex.Message}");
            }
        }

        public async Task<Review?> UpdateReview(ReviewModel reviewModel, string reviewId)
        {
            try
            {
                var review = await GetReviewById(reviewId);

                if (review == null)
                {
                    throw new Exception("Review not found");
                }

                review.UserId = reviewModel.UserId;
                review.ProductId = reviewModel.ProductId;
                review.Rating = reviewModel.Rating;
                review.ReviewText = reviewModel.ReviewText;
                review.ReviewDate = DateTime.Now;

                await _reviewRepository.Update(review);

                return review;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating review: {ex.Message}");
            }
        }

        public async Task DeleteReview(string id)
        {
            try
            {
                await _reviewRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting review: {ex.Message}");
            }
        }
    }
}
