using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class UserDetailModel
    {
        [StringLength(50)]
        public string FullName { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(int.MaxValue)]
        public string? ProfilePicture { get; set; }

        public IFormFile? ImageUrl { get; set; }
    }

    public class UserDetailModelValidator : AbstractValidator<UserDetailModel>
    {
        public UserDetailModelValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full Name is required")
                .MaximumLength(50).WithMessage("Full Name must be less than 50 characters");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address must be less than 500 characters");

            RuleFor(x => x.ProfilePicture)
                .MaximumLength(int.MaxValue).WithMessage("Profile Picture URL length is too long");
        }
    }
}
