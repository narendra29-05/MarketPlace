using Findly.Contracts.Category.Responses;
using Findly.Domain.Enums;

namespace Findly.Contracts.Catalog.Responses;

public class CompareResponse
{
    public List<CompareItemResponse> Listings { get; set; } = [];
}

public class CompareItemResponse
{
    public int         Id                    { get; set; }
    public string      Name                  { get; set; }
    public string      Slug                  { get; set; }
    public string?     LogoUrl               { get; set; }
    public string      WebsiteUrl            { get; set; }
    public PricingType PricingType           { get; set; }
    public decimal?    StartingPrice         { get; set; }
    public decimal?    PricePerUser          { get; set; }
    public bool        HasFreeTrial          { get; set; }
    public int?        FreeTrialDays         { get; set; }
    public decimal     AverageRating         { get; set; }
    public decimal     FeaturesRating        { get; set; }
    public decimal     ValueForMoneyRating   { get; set; }
    public decimal     CustomerSupportRating { get; set; }
    public int         ReviewCount           { get; set; }

    public List<CategoryResponse> Categories { get; set; } = [];
}
