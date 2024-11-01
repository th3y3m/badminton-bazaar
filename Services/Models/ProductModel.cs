using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FluentValidation;

namespace Services.Models
{
    public class ProductModel
    {
        public string ProductName { get; set; }

        public string CategoryId { get; set; }

        public string SupplierId { get; set; }

        public string? ProductDescription { get; set; }

        public decimal BasePrice { get; set; }

        public string Gender { get; set; }

        public string? ImageUrl { get; set; }

        public bool Status { get; set; }

        public IFormFile ProductImageUrl { get; set; }
    }

    public class ProductModelValidator : AbstractValidator<ProductModel>
    {
        public ProductModelValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product Name is required")
                .MaximumLength(50).WithMessage("Product Name must be less than 50 characters");

            RuleFor(x => x.CategoryId)
                .MaximumLength(10).WithMessage("Category ID must be less than 10 characters");

            RuleFor(x => x.SupplierId)
                .MaximumLength(10).WithMessage("Supplier ID must be less than 10 characters");

            RuleFor(x => x.ProductDescription)
                .MaximumLength(500).WithMessage("Product Description must be less than 500 characters");

            RuleFor(x => x.BasePrice)
                .GreaterThanOrEqualTo(0).WithMessage("Base Price must be a positive value");

            RuleFor(x => x.Gender)
                .MaximumLength(10).WithMessage("Gender must be less than 10 characters");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(int.MaxValue).WithMessage("Image URL length is too long");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status must be specified");

            RuleFor(x => x.ProductImageUrl)
                .NotNull().WithMessage("Product Image URL must be specified");
        }
    }
}
