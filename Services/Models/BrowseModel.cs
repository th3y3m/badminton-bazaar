using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models
{
    public class BrowsingData
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public float BrowseCount { get; set; }
    }

    public class ProductRecommendation
    {
        public Product Product { get; set; }
        public float Score { get; set; }
    }

    public class ProductRatingData
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public float Rating { get; set; }
        public float ViewCount { get; set; }
        public string CategoryId { get; set; }
        public string SupplierId { get; set; }
        public float Price { get; set; }
        public DateTime LastAccessed { get; set; }
    }

    public class ProductPrediction
    {
        public float Score { get; set; }
    }

    public class ProductInteraction
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string SupplierId { get; set; }
        public decimal BasePrice { get; set; }
        public string ProductDescription { get; set; }
        public string Gender { get; set; }
        public string ProductName { get; set; }
        public bool Status { get; set; }
        public float Label { get; set; }
    }

    public class ProductInteractionV2
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public float Label { get; set; } // Interaction strength (e.g., view count, rating)
    }

    public class ProductScorePrediction
    {
        public float Score { get; set; }
    }

    public class RatingPrediction
    {
        public float Score { get; set; }
    }

    public class UserProductRating
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public float Rating { get; set; }
    }
}
