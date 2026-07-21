using Findly.Contracts.Review.Requests;
using FluentValidation;

namespace Findly.Contracts.Review.Validators;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.OverallRating).InclusiveBetween(1, 5);
        RuleFor(x => x.FeaturesRating).InclusiveBetween(1, 5);
        RuleFor(x => x.ValueForMoneyRating).InclusiveBetween(1, 5);
        RuleFor(x => x.CustomerSupportRating).InclusiveBetween(1, 5);

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Pros)
            .MaximumLength(2000);

        RuleFor(x => x.Cons)
            .MaximumLength(2000);
    }
}
