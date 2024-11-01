using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class ReviewModel
    {
        public string ReviewText { get; set; }

        public int Rating { get; set; }

        public string UserId { get; set; }

        public string ProductId { get; set; }
    }

    public class ReviewModelValidator : AbstractValidator<ReviewModel>
    {
        public ReviewModelValidator()
        {
            RuleFor(x => x.ReviewText)
                .NotEmpty().WithMessage("Review Text is required")
                .MaximumLength(255).WithMessage("Review Text must be less than 255 characters");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID is required")
                .MaximumLength(450).WithMessage("User ID must be less than 450 characters");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required")
                .MaximumLength(10).WithMessage("Product ID must be less than 10 characters");
        }
    }
}
