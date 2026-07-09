using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class CreateListingRequestValidator : AbstractValidator<CreateListingRequest>
{
    public CreateListingRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200).Matches(@"^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");
        RuleFor(x => x.ShortDescription).NotEmpty().MaximumLength(500);
        RuleFor(x => x.WebsiteUrl).NotEmpty().MaximumLength(500).Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("WebsiteUrl must be a valid URL.");
        RuleFor(x => x.Tagline).MaximumLength(300).When(x => !string.IsNullOrEmpty(x.Tagline));
        RuleFor(x => x.LogoUrl).MaximumLength(500).Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("LogoUrl must be a valid URL.").When(x => !string.IsNullOrEmpty(x.LogoUrl));
        RuleFor(x => x.DemoUrl).MaximumLength(500).Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("DemoUrl must be a valid URL.").When(x => !string.IsNullOrEmpty(x.DemoUrl));
        RuleFor(x => x.VendorId).GreaterThan(0);
        RuleFor(x => x.PricingType).IsInEnum();
        RuleFor(x => x.FreeTrialDays).GreaterThan(0).When(x => x.HasFreeTrial);
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}
