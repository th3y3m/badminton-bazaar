﻿using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services.Helper;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ReviewService
    {
        private readonly ReviewRepository _reviewRepository;

        public ReviewService(ReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public PaginatedList<Review> GetPaginatedReviews(
            string searchQuery,
            string sortBy,
            string userId,
            string productId,
            int? rating,
            int pageIndex,
            int pageSize)
        {
            var source = _reviewRepository.GetDbSet().AsNoTracking();

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

        public Review GetReviewById(string id)
        {
            return _reviewRepository.GetById(id);
        }

        public Review AddReview(ReviewModel reviewModel)
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

            _reviewRepository.Add(review);

            return review;
        }

        public Review UpdateReview(ReviewModel reviewModel, string reviewId)
        {
            var review = GetReviewById(reviewId);

            if (review == null)
            {
                return null;
            }

            review.UserId = reviewModel.UserId;
            review.ProductId = reviewModel.ProductId;
            review.Rating = reviewModel.Rating;
            review.ReviewText = reviewModel.ReviewText;
            review.ReviewDate = DateTime.Now;

            _reviewRepository.Update(review);

            return review;
        }

        public void DeleteReview(string id)
        {
            _reviewRepository.Delete(id);
        }

    }
}
