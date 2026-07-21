using Findly.Contracts.Lead.Requests;
using FluentValidation;

namespace Findly.Contracts.Lead.Validators;

public class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.LeadType)
            .IsInEnum();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BusinessEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Phone)
            .Matches(@"^\+?[1-9]\d{1,14}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone))
            .WithMessage("Phone number format is not valid.");

        RuleFor(x => x.Company)
            .MaximumLength(200);

        RuleFor(x => x.CompanySize)
            .GreaterThan(0)
            .When(x => x.CompanySize.HasValue);

        RuleFor(x => x.Message)
            .MaximumLength(2000);
    }
}
