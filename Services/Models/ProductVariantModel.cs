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
    public class ProductVariantModel
    {
        public string ProductId { get; set; }

        public string ColorId { get; set; }

        public string SizeId { get; set; }

        public int StockQuantity { get; set; }

        public string? VariantImageURL { get; set; } = null;

        public decimal Price { get; set; }

        public bool Status { get; set; }

        public List<IFormFile> ProductImageUrl { get; set; }
    }

    public class ProductVariantModelValidator : AbstractValidator<ProductVariantModel>
    {
        public ProductVariantModelValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required")
                .MaximumLength(10).WithMessage("Product ID must be less than 10 characters");

            RuleFor(x => x.ColorId)
                .MaximumLength(10).WithMessage("Color ID must be less than 10 characters");

            RuleFor(x => x.SizeId)
                .MaximumLength(10).WithMessage("Size ID must be less than 10 characters");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock Quantity must be a positive value");

            RuleFor(x => x.VariantImageURL)
                .MaximumLength(int.MaxValue).WithMessage("Variant Image URL length is too long");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be a positive value");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status must be specified");

            RuleFor(x => x.ProductImageUrl)
                .NotEmpty().WithMessage("Product Image URL must be specified");
        }
    }
}
