using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
{
    public CreateTagRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(150)
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}
