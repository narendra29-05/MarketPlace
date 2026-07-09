using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(200);
        RuleFor(x => x.Role).IsInEnum();
        RuleFor(x => x.JobTitle).MaximumLength(150).When(x => !string.IsNullOrEmpty(x.JobTitle));
        RuleFor(x => x.CompanyName).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.CompanyName));
        RuleFor(x => x.IndustryType).IsInEnum().When(x => x.IndustryType.HasValue);
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}
