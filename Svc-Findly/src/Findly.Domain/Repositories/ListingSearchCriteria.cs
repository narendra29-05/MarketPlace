using Findly.Domain.Enums;

namespace Findly.Domain.Repositories;

public sealed record ListingSearchCriteria(
    string?       Search,
    int?          CategoryId,
    string?       CategorySlug,
    PricingType?  PricingType,
    decimal?      MinRating,
    bool?         HasFreeTrial,
    ListingSortBy SortBy,
    int           Page,
    int           PageSize);
