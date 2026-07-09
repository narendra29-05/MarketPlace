using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class CreateVendorRequestValidator : AbstractValidator<CreateVendorRequest>
{
    public CreateVendorRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number format is not valid.");
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CompanySize).GreaterThan(0).When(x => x.CompanySize.HasValue);
        RuleFor(x => x.Industry).IsInEnum();
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}
