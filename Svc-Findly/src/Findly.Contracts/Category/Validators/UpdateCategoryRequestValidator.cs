using Findly.Contracts.Category.Requests;
using FluentValidation;

namespace Findly.Contracts.Category.Validators;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[a-z0-9-]+$")
            .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.IconUrl)
            .MaximumLength(500);
    }
}
