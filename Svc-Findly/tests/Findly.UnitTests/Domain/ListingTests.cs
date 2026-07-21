using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Domain;

public class ListingTests
{
    private static Listing NewListing(bool hasFreeTrial = false, int? freeTrialDays = null) => Listing.Create(
        "AcmeCRM", "acme-crm", "A friendly CRM", "https://acme.io", 1, PricingType.Paid, "test",
        "CRM for SMBs", null, null, null, null, 29m, 9m, hasFreeTrial, freeTrialDays);

    [Fact]
    public void Create_starts_pending_with_zero_ratings()
    {
        var listing = NewListing();

        Assert.Equal(ListingStatus.Pending, listing.Status);
        Assert.Equal(0, listing.AverageRating);
        Assert.Equal(0, listing.ReviewCount);
    }

    [Fact]
    public void Create_without_free_trial_clears_trial_days()
    {
        var listing = NewListing(hasFreeTrial: false, freeTrialDays: 14);

        Assert.Null(listing.FreeTrialDays);
    }

    [Fact]
    public void Approve_pending_publishes()
    {
        var listing = NewListing();

        listing.Approve("admin");

        Assert.Equal(ListingStatus.Published, listing.Status);
    }

    [Fact]
    public void Approve_published_throws()
    {
        var listing = NewListing();
        listing.Approve("admin");

        Assert.Throws<InvalidOperationException>(() => listing.Approve("admin"));
    }

    [Fact]
    public void UpdateDetails_on_pending_stays_pending()
    {
        var listing = NewListing();

        listing.UpdateDetails("New", "new-slug", "desc", "https://x.io", "v", null, null, null, null, null);

        Assert.Equal(ListingStatus.Pending, listing.Status);
        Assert.Equal("New", listing.Name);
    }

    [Fact]
    public void UpdateDetails_on_published_returns_to_pending()
    {
        var listing = NewListing();
        listing.Approve("admin");

        listing.UpdateDetails("New", "new-slug", "desc", "https://x.io", "v", null, null, null, null, null);

        Assert.Equal(ListingStatus.Pending, listing.Status);
    }

    [Fact]
    public void UpdateDetails_on_rejected_returns_to_pending_and_clears_reason()
    {
        var listing = NewListing();
        listing.Reject("bad description", "admin");

        listing.UpdateDetails("New", "new-slug", "desc", "https://x.io", "v", null, null, null, null, null);

        Assert.Equal(ListingStatus.Pending, listing.Status);
        Assert.Null(listing.RejectionReason);
    }

    [Fact]
    public void UpdateDetails_on_archived_throws()
    {
        var listing = NewListing();
        listing.Approve("admin");
        listing.Archive("v");

        Assert.Throws<InvalidOperationException>(() =>
            listing.UpdateDetails("New", "new-slug", "desc", "https://x.io", "v", null, null, null, null, null));
    }

    [Fact]
    public void Archive_requires_published()
    {
        var listing = NewListing();

        Assert.Throws<InvalidOperationException>(() => listing.Archive("v"));
    }

    [Fact]
    public void Restore_archived_returns_to_pending()
    {
        var listing = NewListing();
        listing.Approve("admin");
        listing.Archive("v");

        listing.Restore("v");

        Assert.Equal(ListingStatus.Pending, listing.Status);
    }

    [Fact]
    public void UpdateRatings_sets_aggregates()
    {
        var listing = NewListing();

        listing.UpdateRatings(4.50m, 4.00m, 5.00m, 3.50m, 7);

        Assert.Equal(4.50m, listing.AverageRating);
        Assert.Equal(4.00m, listing.FeaturesRating);
        Assert.Equal(5.00m, listing.ValueForMoneyRating);
        Assert.Equal(3.50m, listing.CustomerSupportRating);
        Assert.Equal(7, listing.ReviewCount);
    }
}
