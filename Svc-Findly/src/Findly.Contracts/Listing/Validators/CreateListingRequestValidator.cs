using Findly.Contracts.Listing.Requests;
using FluentValidation;

namespace Findly.Contracts.Listing.Validators;

public class CreateListingRequestValidator :AbstractValidator<CreateListingRequest>
{
   public CreateListingRequestValidator()
      {
          RuleFor(x => x.Name)
              .NotEmpty()
              .MaximumLength(200);

          RuleFor(x => x.Slug)
              .NotEmpty()
              .MaximumLength(200)
              .Matches(@"^[a-z0-9-]+$")
              .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");

          RuleFor(x => x.ShortDescription)
              .NotEmpty()
              .MaximumLength(500);

          RuleFor(x => x.WebsiteUrl)
              .NotEmpty()
              .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
              .WithMessage("WebsiteUrl must be a valid URL.");

          RuleFor(x => x.VendorId)
              .GreaterThan(0);

          RuleFor(x => x.PricingType)
              .IsInEnum();
  
          RuleFor(x => x.FreeTrialDays)
              .GreaterThan(0)
              .When(x => x.HasFreeTrial);
      }
  }
