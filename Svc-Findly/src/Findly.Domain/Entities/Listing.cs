using Findly.Domain.Enums;
using Findly.Domain.Entities;

namespace Findly.Domain.Entities;

public class Listing : Entity
{
    protected Listing() { }

    // Identity
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Tagline { get; private set; }
    public string ShortDescription { get; private set; }
    public string? Description { get; private set; }
    public string? LogoUrl { get; private set; }
    public string WebsiteUrl { get; private set; }
    public string? DemoUrl { get; private set; }
    public int? FoundedYear { get; private set; }

    // Vendor
    public int VendorId { get; private set; }

    // Pricing

    public PricingType PricingType { get; private set; }
    public decimal? StartingPrice { get; private set; }
    public decimal? PricePerUser { get; private set; }
    public bool HasFreeTrial { get; private set; }
    public int? FreeTrialDays { get; private set; }

    // Ratings
    public decimal AverageRating { get; private set; }
    public decimal FeaturesRating { get; private set; }
    public decimal ValueForMoneyRating { get; private set; }
    public decimal CustomerSupportRating { get; private set; }
    public int ReviewCount { get; private set; }

    // Status
    public ListingStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Listing Create(
        string name,
        string slug,
        string shortDescription,
        string websiteUrl,
        int vendorId,
        PricingType pricingType,
        string createdBy,
        string? tagline,
        string? description,
        string? logoUrl,
        string? demoUrl,
        int? foundedYear,
        decimal? startingPrice,
        decimal? pricePerUser,
        bool hasFreeTrial,
        int? freeTrialDays)
    {
        return new Listing
        {
            Name = name,
            Slug = slug,
            Tagline = tagline,
            ShortDescription = shortDescription,
            Description = description,
            LogoUrl = logoUrl,
            WebsiteUrl = websiteUrl,
            DemoUrl = demoUrl,
            FoundedYear = foundedYear,
            VendorId = vendorId,
            PricingType = pricingType,
            StartingPrice = startingPrice,
            PricePerUser = pricePerUser,
            HasFreeTrial = hasFreeTrial,
            FreeTrialDays = hasFreeTrial ? freeTrialDays : null,
            AverageRating = 0,
            FeaturesRating = 0,
            ValueForMoneyRating = 0,
            CustomerSupportRating = 0,
            ReviewCount = 0,
            Status = ListingStatus.Pending,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateDetails(
        string name,
        string slug,
        string shortDescription,
        string websiteUrl,
        string updatedBy,
        string? tagline,
        string? description,
        string? logoUrl,
        string? demoUrl,
        int? foundedYear)
    {
        if (Status == ListingStatus.Archived)
            throw new InvalidOperationException("Archived listings cannot be edited. Restore the listing first.");

        Name = name;
        Slug = slug;
        Tagline = tagline;
        ShortDescription = shortDescription;
        Description = description;
        LogoUrl = logoUrl;
        WebsiteUrl = websiteUrl;
        DemoUrl = demoUrl;
        FoundedYear = foundedYear;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;

        // Edits to rejected or published listings go back through moderation.
        if (Status is ListingStatus.Rejected or ListingStatus.Published)
        {
            Status = ListingStatus.Pending;
            RejectionReason = null;
        }
    }

    public void UpdatePricing(
        PricingType pricingType,
        string updatedBy,
        decimal? startingPrice,
        decimal? pricePerUser,
        bool hasFreeTrial = false,
        int? freeTrialDays = null)
    {
        PricingType = pricingType;
        StartingPrice = startingPrice;
        PricePerUser = pricePerUser;
        HasFreeTrial = hasFreeTrial;
        FreeTrialDays = hasFreeTrial ? freeTrialDays : null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateRatings(
        decimal averageRating,
        decimal featuresRating,
        decimal valueForMoneyRating,
        decimal customerSupportRating,
        int reviewCount)
    {
        AverageRating = averageRating;
        FeaturesRating = featuresRating;
        ValueForMoneyRating = valueForMoneyRating;
        CustomerSupportRating = customerSupportRating;
        ReviewCount = reviewCount;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(string updatedBy)
    {
        if (Status != ListingStatus.Pending)
            throw new InvalidOperationException($"Cannot approve from status '{Status}'.");

        Status = ListingStatus.Published;
        RejectionReason = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Reject(string reason, string updatedBy)
    {
        if (Status != ListingStatus.Pending)
            throw new InvalidOperationException($"Cannot reject from status '{Status}'.");

        Status = ListingStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Archive(string updatedBy)
    {
        if (Status != ListingStatus.Published)
            throw new InvalidOperationException("Only published listings can be archived.");

        Status = ListingStatus.Archived;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Restore(string updatedBy)
    {
        if (Status != ListingStatus.Archived)
            throw new InvalidOperationException("Only archived listings can be restored.");

        Status = ListingStatus.Pending;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
