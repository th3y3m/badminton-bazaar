using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class UnlinkExternalLoginRequest
    {
        public string Email { get; set; }
        public string Provider { get; set; }
    }

    public class UnlinkExternalLoginRequestValidator : AbstractValidator<UnlinkExternalLoginRequest>
    {
        public UnlinkExternalLoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid Email Address");

            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required");
        }
    }
}
