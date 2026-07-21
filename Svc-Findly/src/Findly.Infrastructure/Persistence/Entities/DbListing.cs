using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Listings")]
internal class DbListing
{
    [Key]
    public int           Id                    { get; set; }
    public string        Name                  { get; set; } 
    public string        Slug                  { get; set; } 
    public string?       Tagline               { get; set; }
    public string        ShortDescription      { get; set; } 
    public string?       Description           { get; set; }
    public string?       LogoUrl               { get; set; }
    public string        WebsiteUrl            { get; set; } 
    public string?       DemoUrl               { get; set; }
    public int?          FoundedYear           { get; set; }
    public int           VendorId              { get; set; }
    public PricingType   PricingType           { get; set; }
    public decimal?      StartingPrice         { get; set; }
    public decimal?      PricePerUser          { get; set; }
    public bool          HasFreeTrial          { get; set; }
    public int?          FreeTrialDays         { get; set; }
    public decimal       AverageRating         { get; set; }
    public decimal       FeaturesRating        { get; set; }
    public decimal       ValueForMoneyRating   { get; set; }
    public decimal       CustomerSupportRating { get; set; }
    public int           ReviewCount           { get; set; }
    public ListingStatus Status                { get; set; }
    public string?       RejectionReason       { get; set; }
    public string?       CreatedBy             { get; set; }
    public DateTime      CreatedAt             { get; set; }
    public string?       UpdatedBy             { get; set; }
    public DateTime      UpdatedAt             { get; set; }
}
