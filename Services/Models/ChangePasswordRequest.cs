using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class ChangePasswordRequest
    {
        public string Mail { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.Mail)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current Password is required");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New Password is required");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm Password is required")
                .Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match");
        }
    }
}
