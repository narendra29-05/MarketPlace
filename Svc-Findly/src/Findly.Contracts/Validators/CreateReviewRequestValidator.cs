using Findly.Contracts.Requests;
using FluentValidation;

namespace Findly.Contracts.Validators;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.ListingId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.OverallRating).InclusiveBetween(1, 5);
        RuleFor(x => x.FeaturesRating).InclusiveBetween(1, 5);
        RuleFor(x => x.CustomerSupportRating).InclusiveBetween(1, 5);
        RuleFor(x => x.Pros).MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Pros));
        RuleFor(x => x.Cons).MaximumLength(2000).When(x => !string.IsNullOrEmpty(x.Cons));
        RuleFor(x => x.CreatedBy).NotEmpty().MaximumLength(100);
    }
}
