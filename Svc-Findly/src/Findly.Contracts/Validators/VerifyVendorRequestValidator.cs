using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class VerifyVendorRequestValidator : AbstractValidator<VerifyVendorRequest>
{
    public VerifyVendorRequestValidator()
    {
        RuleFor(x => x.UpdatedBy).NotEmpty().MaximumLength(100);
    }
}
