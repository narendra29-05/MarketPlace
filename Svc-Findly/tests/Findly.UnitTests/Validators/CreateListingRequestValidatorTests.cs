using Findly.Contracts.Listing.Requests;
using Findly.Contracts.Listing.Validators;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Validators;

public class CreateListingRequestValidatorTests
{
    private readonly CreateListingRequestValidator _validator = new();

    private static CreateListingRequest ValidRequest() => new()
    {
        Name = "AcmeCRM",
        Slug = "acme-crm",
        ShortDescription = "A friendly CRM",
        WebsiteUrl = "https://acme.io",
        PricingType = PricingType.Paid,
        CategoryIds = [1, 2]
    };

    [Fact]
    public void Valid_request_passes()
    {
        Assert.True(_validator.Validate(ValidRequest()).IsValid);
    }

    [Fact]
    public void Empty_categories_fail()
    {
        var request = ValidRequest();
        request.CategoryIds = [];

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void More_than_five_categories_fail()
    {
        var request = ValidRequest();
        request.CategoryIds = [1, 2, 3, 4, 5, 6];

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Duplicate_categories_fail()
    {
        var request = ValidRequest();
        request.CategoryIds = [1, 1];

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Theory]
    [InlineData("Has Spaces")]
    [InlineData("UPPER")]
    [InlineData("bad_underscore")]
    public void Invalid_slug_fails(string slug)
    {
        var request = ValidRequest();
        request.Slug = slug;

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Free_trial_requires_positive_days()
    {
        var request = ValidRequest();
        request.HasFreeTrial = true;
        request.FreeTrialDays = 0;

        Assert.False(_validator.Validate(request).IsValid);
    }
}
