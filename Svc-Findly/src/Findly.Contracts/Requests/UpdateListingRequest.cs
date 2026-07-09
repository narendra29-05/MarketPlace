using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

public class UpdateListingRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string ShortDescription { get; set; } = null!;
    public string WebsiteUrl { get; set; } = null!;
    public PricingType PricingType { get; set; }
    public string? Tagline { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? DemoUrl { get; set; }
    public int? FoundedYear { get; set; }
    public decimal? StartingPrice { get; set; }
    public decimal? PricePerUser { get; set; }
    public bool HasFreeTrial { get; set; }
    public int? FreeTrialDays { get; set; }
    public string UpdatedBy { get; set; } = null!;
}
