using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class NewsModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public string Image { get; set; }

        public bool IsHomepageSlideshow { get; set; }

        public bool IsHomepageBanner { get; set; }

        public bool Status { get; set; }
    }

    public class NewsModelValidator : AbstractValidator<NewsModel>
    {
        public NewsModelValidator()
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("Title must be specified")
                .MaximumLength(255).WithMessage("Title must be less than 255 characters");

            RuleFor(x => x.Content)
                .MaximumLength(int.MaxValue).WithMessage("Content length is too long");

            RuleFor(x => x.Image)
                .MaximumLength(int.MaxValue).WithMessage("Image length is too long");

            RuleFor(x => x.IsHomepageSlideshow)
                .NotNull().WithMessage("IsHomepageSlideshow must be specified");

            RuleFor(x => x.IsHomepageBanner)
                .NotNull().WithMessage("IsHomepageBanner must be specified");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status must be specified");
        }
    }
}
