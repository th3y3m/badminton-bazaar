using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class SizeModel
    {
        public string SizeId { get; set; }

        public string SizeName { get; set; }
    }

    public class SizeModelValidator : AbstractValidator<SizeModel>
    {
        public SizeModelValidator()
        {
            RuleFor(x => x.SizeId)
                .NotEmpty().WithMessage("Size ID is required")
                .MaximumLength(10).WithMessage("Size ID must be less than 10 characters");

            RuleFor(x => x.SizeName)
                .NotEmpty().WithMessage("Size Name is required")
                .MaximumLength(10).WithMessage("Size Name must be less than 10 characters");
        }
    }
}
