using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Services.Models
{
    public class SupplierModel
    {
        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public bool Status { get; set; }
    }

    public class SupplierModelValidator : AbstractValidator<SupplierModel>
    {
        public SupplierModelValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company Name is required")
                .MaximumLength(100).WithMessage("Company Name must be less than 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be less than 500 characters");

            RuleFor(x => x.Address)
                .MaximumLength(500).WithMessage("Address must be less than 500 characters");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status must be specified");
        }
    }
}
