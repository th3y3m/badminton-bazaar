using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class ColorModel
    {
        [StringLength(10)]
        public string ColorId { get; set; }

        [StringLength(50)]
        public string ColorName { get; set; }
    }

    public class ColorModelValidator : AbstractValidator<ColorModel>
    {
        public ColorModelValidator()
        {
            RuleFor(x => x.ColorId)
                .NotEmpty().WithMessage("Color ID is required");

            RuleFor(x => x.ColorName)
                .NotEmpty().WithMessage("Color Name is required");
        }
    }
}
