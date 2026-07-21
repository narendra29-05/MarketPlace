using Findly.Contracts.Catalog.Requests;
using FluentValidation;

namespace Findly.Contracts.Catalog.Validators;

public class CatalogSearchRequestValidator : AbstractValidator<CatalogSearchRequest>
{
    public CatalogSearchRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(0, 5)
            .When(x => x.MinRating.HasValue);

        RuleFor(x => x.SortBy)
            .IsInEnum();

        RuleFor(x => x.PricingType)
            .IsInEnum()
            .When(x => x.PricingType.HasValue);
    }
}
