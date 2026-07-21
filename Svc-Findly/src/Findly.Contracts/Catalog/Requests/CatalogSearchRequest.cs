using Findly.Domain.Enums;

namespace Findly.Contracts.Catalog.Requests;

public class CatalogSearchRequest
{
    public string?       Search       { get; set; }
    public int?          CategoryId   { get; set; }
    public string?       CategorySlug { get; set; }
    public PricingType?  PricingType  { get; set; }
    public decimal?      MinRating    { get; set; }
    public bool?         HasFreeTrial { get; set; }
    public ListingSortBy SortBy       { get; set; } = ListingSortBy.Rating;
    public int           Page         { get; set; } = 1;
    public int           PageSize     { get; set; } = 20;
}
