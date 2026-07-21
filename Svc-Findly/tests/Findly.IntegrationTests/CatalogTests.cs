using System.Net;

namespace Findly.IntegrationTests;

[Collection("api")]
public class CatalogTests
{
    private readonly ApiFactory _factory;

    public CatalogTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Categories_are_public_and_seeded()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/catalog/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.True(json.GetArrayLength() >= 12);
    }

    [Fact]
    public async Task Search_is_public_and_paged()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/catalog/listings?page=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.ReadJsonAsync();
        Assert.True(json.TryGetProperty("totalCount", out _));
        Assert.True(json.TryGetProperty("items", out _));
    }

    [Fact]
    public async Task Compare_with_single_id_is_bad_request()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/catalog/compare?ids=1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Oversized_page_size_is_rejected()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/catalog/listings?pageSize=500");

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_slug_is_404_problem_details()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/catalog/listings/definitely-not-a-slug");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
