using Findly.Domain.Enums;

namespace Findly.Contracts.Listing.Requests;

public class CreateListingRequest
{
    public string      Name             { get; set; }
    public string      Slug             { get; set; }
    public string      ShortDescription { get; set; }
    public string      WebsiteUrl       { get; set; }
    public PricingType PricingType      { get; set; }
    public List<int>   CategoryIds      { get; set; } = [];
    public string?     Tagline          { get; set; }
    public string?     Description      { get; set; }
    public string?     LogoUrl          { get; set; }
    public string?     DemoUrl          { get; set; }
    public int?        FoundedYear      { get; set; }
    public decimal?    StartingPrice    { get; set; }
    public decimal?    PricePerUser     { get; set; }
    public bool        HasFreeTrial     { get; set; }
    public int?        FreeTrialDays    { get; set; }
}
