using System.Net;
using System.Net.Http.Json;

namespace Findly.IntegrationTests;

[Collection("api")]
public class MarketplaceFlowTests
{
    private readonly ApiFactory _factory;

    public MarketplaceFlowTests(ApiFactory factory) => _factory = factory;

    /// <summary>
    /// The whole product journey with real auth: vendor registers and onboards →
    /// admin verifies → listing → approval → buyer review → aggregates →
    /// anonymous lead → vendor pipeline.
    /// </summary>
    [Fact]
    public async Task Full_vendor_to_lead_journey()
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        var admin  = _factory.CreateClient();
        var vendor = _factory.CreateClient();
        var buyer  = _factory.CreateClient();
        var anon   = _factory.CreateClient();

        admin.WithToken(await admin.LoginAsync("admin@findly.local", "Admin@123!"));
        var (vendorToken, vendorEmail) = await vendor.RegisterAsync(role: 2, prefix: "vendor");
        vendor.WithToken(vendorToken);
        var (buyerToken, _) = await buyer.RegisterAsync(role: 3, prefix: "buyer");
        buyer.WithToken(buyerToken);

        // --- vendor onboarding (linked to the signed-in account) ---
        var vendorProfile = await vendor.PostAsJsonAsync("/api/v1/vendors", new
        {
            firstName = "Vera", lastName = "Vendor", email = vendorEmail, phone = "+14155550101",
            companyName = $"Acme {suffix}", companySize = 50, industry = 2,
            addressLine1 = "1 Main St", addressLine2 = "", city = "Austin",
            state = "TX", country = "USA", postalCode = "73301"
        });
        Assert.Equal(HttpStatusCode.Created, vendorProfile.StatusCode);
        var vendorId = (await vendorProfile.ReadJsonAsync()).GetProperty("id").GetInt32();

        // listing before verification is blocked
        var early = await vendor.PostAsJsonAsync("/api/v1/listings", ListingBody(suffix));
        Assert.Equal(HttpStatusCode.Conflict, early.StatusCode);

        await admin.PostAsync($"/api/v1/vendors/{vendorId}/verify", null);

        // --- listing + approval (vendor resolved from the token) ---
        var listingResponse = await vendor.PostAsJsonAsync("/api/v1/listings", ListingBody(suffix));
        Assert.Equal(HttpStatusCode.Created, listingResponse.StatusCode);
        var listingId = (await listingResponse.ReadJsonAsync()).GetProperty("id").GetInt32();

        // buyer cannot approve
        var forbidden = await buyer.PostAsync($"/api/v1/listings/{listingId}/approve", null);
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);

        await admin.PostAsync($"/api/v1/listings/{listingId}/approve", null);

        // --- review: identity comes from the buyer's account ---
        var reviewResponse = await buyer.PostAsJsonAsync($"/api/v1/listings/{listingId}/reviews", new
        {
            overallRating = 5, featuresRating = 4, valueForMoneyRating = 5, customerSupportRating = 4,
            title = "Great", body = "Solid tool."
        });
        Assert.Equal(HttpStatusCode.Created, reviewResponse.StatusCode);
        var reviewId = (await reviewResponse.ReadJsonAsync()).GetProperty("id").GetInt32();

        // same account cannot review twice
        var duplicate = await buyer.PostAsJsonAsync($"/api/v1/listings/{listingId}/reviews", new
        {
            overallRating = 4, featuresRating = 4, valueForMoneyRating = 4, customerSupportRating = 4,
            title = "Again", body = "x"
        });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        // vendor cannot review at all
        var vendorReview = await vendor.PostAsJsonAsync($"/api/v1/listings/{listingId}/reviews", new
        {
            overallRating = 5, featuresRating = 5, valueForMoneyRating = 5, customerSupportRating = 5,
            title = "Self", body = "x"
        });
        Assert.Equal(HttpStatusCode.Forbidden, vendorReview.StatusCode);

        await admin.PostAsync($"/api/v1/reviews/{reviewId}/approve", null);

        var aggregated = await (await anon.GetAsync($"/api/v1/catalog/listings/crm-{suffix}")).ReadJsonAsync();
        Assert.Equal(1, aggregated.GetProperty("reviewCount").GetInt32());
        Assert.Equal(5.00m, aggregated.GetProperty("averageRating").GetDecimal());

        // --- anonymous lead → vendor pipeline (identity-based) ---
        var leadResponse = await anon.PostAsJsonAsync($"/api/v1/listings/{listingId}/leads", new
        {
            leadType = 2, fullName = "Jane CTO", businessEmail = "jane@bigco.com",
            company = "BigCo", message = "Demo please"
        });
        Assert.Equal(HttpStatusCode.Created, leadResponse.StatusCode);
        var leadId = (await leadResponse.ReadJsonAsync()).GetProperty("id").GetInt32();

        var pipeline = await (await vendor.GetAsync("/api/v1/vendor-portal/leads?status=1")).ReadJsonAsync();
        Assert.True(pipeline.GetProperty("totalCount").GetInt32() >= 1);

        var contacted = await vendor.PostAsJsonAsync($"/api/v1/vendor-portal/leads/{leadId}/status", new { status = 2 });
        Assert.Equal(HttpStatusCode.OK, contacted.StatusCode);

        var invalidJump = await vendor.PostAsJsonAsync($"/api/v1/vendor-portal/leads/{leadId}/status", new { status = 4 });
        Assert.Equal(HttpStatusCode.Conflict, invalidJump.StatusCode);

        // buyer cannot read the vendor pipeline
        var buyerLeads = await buyer.GetAsync("/api/v1/vendor-portal/leads");
        Assert.Equal(HttpStatusCode.Forbidden, buyerLeads.StatusCode);
    }

    private static object ListingBody(string suffix) => new
    {
        name = $"CRM {suffix}",
        slug = $"crm-{suffix}",
        shortDescription = "A friendly CRM",
        websiteUrl = "https://acme.io",
        pricingType = 3,
        categoryIds = new[] { 1 },
        hasFreeTrial = true,
        freeTrialDays = 14
    };
}
