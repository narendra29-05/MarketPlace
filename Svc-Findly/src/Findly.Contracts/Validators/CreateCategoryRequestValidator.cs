using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(150)
            .Matches(@"^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");
        RuleFor(x => x.Description).MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Description));
        RuleFor(x => x.IconUrl).MaximumLength(500)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("IconUrl must be a valid URL.")
            .When(x => !string.IsNullOrEmpty(x.IconUrl));
        RuleFor(x => x.ParentCategoryId).GreaterThan(0).When(x => x.ParentCategoryId.HasValue);
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}
