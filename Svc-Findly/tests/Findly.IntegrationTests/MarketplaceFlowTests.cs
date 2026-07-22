using System.Net;
using System.Net.Http.Json;

namespace Findly.IntegrationTests;

[Collection("api")]
public class MarketplaceFlowTests
{
    private readonly ApiFactory _factory;

    public MarketplaceFlowTests(ApiFactory factory) => _factory = factory;

    /// <summary>
    /// The whole product journey without auth — every request runs as the fixed
    /// local identity: vendor onboarding → verification → listing → approval →
    /// review → aggregates → anonymous lead → vendor pipeline.
    /// The vendor profile is created once and reused on later runs.
    /// </summary>
    [Fact]
    public async Task Full_vendor_to_lead_journey()
    {
        var suffix = Guid.NewGuid().ToString("N")[..10];
        var client = _factory.CreateClient();

        // --- vendor onboarding (first run creates + links; later runs reuse) ---
        var vendorId = await EnsureVendorProfileAsync(client, suffix);

        var vendorJson = await (await client.GetAsync($"/api/v1/vendors/{vendorId}")).ReadJsonAsync();
        if (vendorJson.GetProperty("status").GetInt32() == 1) // still pending
        {
            // listing before verification is blocked
            var early = await client.PostAsJsonAsync("/api/v1/listings", ListingBody(suffix));
            Assert.Equal(HttpStatusCode.Conflict, early.StatusCode);

            await client.PostAsync($"/api/v1/vendors/{vendorId}/verify", null);
        }

        // --- listing + approval ---
        var listingResponse = await client.PostAsJsonAsync("/api/v1/listings", ListingBody(suffix));
        Assert.Equal(HttpStatusCode.Created, listingResponse.StatusCode);
        var listingId = (await listingResponse.ReadJsonAsync()).GetProperty("id").GetInt32();

        await client.PostAsync($"/api/v1/listings/{listingId}/approve", null);

        // --- review: identity comes from the fixed local user ---
        var reviewResponse = await client.PostAsJsonAsync($"/api/v1/listings/{listingId}/reviews", new
        {
            overallRating = 5,
            featuresRating = 4,
            valueForMoneyRating = 5,
            customerSupportRating = 4,
            title = "Great",
            body = "Solid tool."
        });
        Assert.Equal(HttpStatusCode.Created, reviewResponse.StatusCode);
        var reviewId = (await reviewResponse.ReadJsonAsync()).GetProperty("id").GetInt32();

        // same identity cannot review the same listing twice
        var duplicate = await client.PostAsJsonAsync($"/api/v1/listings/{listingId}/reviews", new
        {
            overallRating = 4,
            featuresRating = 4,
            valueForMoneyRating = 4,
            customerSupportRating = 4,
            title = "Again",
            body = "x"
        });
        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);

        await client.PostAsync($"/api/v1/reviews/{reviewId}/approve", null);

        var aggregated = await (await client.GetAsync($"/api/v1/catalog/listings/crm-{suffix}")).ReadJsonAsync();
        Assert.Equal(1, aggregated.GetProperty("reviewCount").GetInt32());
        Assert.Equal(5.00m, aggregated.GetProperty("averageRating").GetDecimal());

        // --- anonymous lead → vendor pipeline ---
        var leadResponse = await client.PostAsJsonAsync($"/api/v1/listings/{listingId}/leads", new
        {
            leadType = 2,
            fullName = "Jane CTO",
            businessEmail = "jane@bigco.com",
            company = "BigCo",
            message = "Demo please"
        });
        Assert.Equal(HttpStatusCode.Created, leadResponse.StatusCode);
        var leadId = (await leadResponse.ReadJsonAsync()).GetProperty("id").GetInt32();

        var pipeline = await (await client.GetAsync("/api/v1/vendor-portal/leads?status=1")).ReadJsonAsync();
        Assert.True(pipeline.GetProperty("totalCount").GetInt32() >= 1);

        var contacted = await client.PostAsJsonAsync($"/api/v1/vendor-portal/leads/{leadId}/status", new { status = 2 });
        Assert.Equal(HttpStatusCode.OK, contacted.StatusCode);

        var invalidJump = await client.PostAsJsonAsync($"/api/v1/vendor-portal/leads/{leadId}/status", new { status = 4 });
        Assert.Equal(HttpStatusCode.Conflict, invalidJump.StatusCode);
    }

    /// <summary>Returns the local user's vendor id, creating the profile on first run.</summary>
    private static async Task<int> EnsureVendorProfileAsync(HttpClient client, string suffix)
    {
        var existing = await client.GetAsync("/api/v1/vendor-portal/profile");
        if (existing.IsSuccessStatusCode)
            return (await existing.ReadJsonAsync()).GetProperty("id").GetInt32();

        var created = await client.PostAsJsonAsync("/api/v1/vendors", new
        {
            firstName = "Vera",
            lastName = "Vendor",
            email = $"vendor-{suffix}@test.findly.local",
            phone = "+14155550101",
            companyName = $"Acme {suffix}",
            companySize = 50,
            industry = 2,
            addressLine1 = "1 Main St",
            addressLine2 = "",
            city = "Austin",
            state = "TX",
            country = "USA",
            postalCode = "73301"
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        return (await created.ReadJsonAsync()).GetProperty("id").GetInt32();
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
