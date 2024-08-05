using BusinessObjects;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IReviewService
    {
        Task<PaginatedList<Review>> GetPaginatedReviews(
            string searchQuery,
            string sortBy,
            string userId,
            string productId,
            int? rating,
            int pageIndex,
            int pageSize);
        Task<Review> GetReviewById(string id);
        Task<Review> AddReview(ReviewModel reviewModel);
        Task<Review?> UpdateReview(ReviewModel reviewModel, string reviewId);
        Task DeleteReview(string id);
        Task<double?> GetAverageRating(string productId);
    }
}
