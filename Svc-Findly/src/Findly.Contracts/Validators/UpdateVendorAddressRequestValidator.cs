using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class UpdateVendorAddressRequestValidator : AbstractValidator<UpdateVendorAddressRequest>
{
    public UpdateVendorAddressRequestValidator()
    {
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AddressLine2).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.AddressLine2));
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.UpdatedBy).NotEmpty().MaximumLength(100);
    }
}
