using Findly.Contracts.Lead.Requests;
using FluentValidation;

namespace Findly.Contracts.Lead.Validators;

public class UpdateLeadStatusRequestValidator : AbstractValidator<UpdateLeadStatusRequest>
{
    public UpdateLeadStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
