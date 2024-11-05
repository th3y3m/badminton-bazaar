using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class ForgetPasswordModel
    {
        public string Email { get; set; }
    }

    public class ResetPasswordModelValidator : AbstractValidator<ForgetPasswordModel>
    {
        public ResetPasswordModelValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address");
        }
    }
}
