using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class RejectVendorRequestValidator : AbstractValidator<RejectVendorRequest>
{
    public RejectVendorRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.UpdatedBy).NotEmpty().MaximumLength(100);
    }
}
