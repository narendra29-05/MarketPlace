using System.Net;
using System.Net.Http.Json;

namespace Findly.IntegrationTests;

[Collection("api")]
public class AuthFlowTests
{
    private readonly ApiFactory _factory;

    public AuthFlowTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_buyer_returns_token_and_me_works()
    {
        var client = _factory.CreateClient();

        var (token, email) = await client.RegisterAsync(role: 3, prefix: "buyer");

        Assert.NotEmpty(token);

        var me   = await client.WithToken(token).GetAsync("/api/v1/auth/me");
        var json = await me.ReadJsonAsync();
        Assert.Equal(email, json.GetProperty("email").GetString());
        Assert.Equal(3, json.GetProperty("role").GetInt32());
    }

    [Fact]
    public async Task Register_admin_role_is_blocked_with_422()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            firstName = "Evil",
            lastName  = "Admin",
            email     = $"evil-{Guid.NewGuid():N}@test.findly.local",
            password  = "Evil@12345",
            role      = 1
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task Register_duplicate_email_conflicts()
    {
        var client = _factory.CreateClient();
        var (_, email) = await client.RegisterAsync(role: 3, prefix: "dup");

        var second = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            firstName = "Test", lastName = "Dup", email, password = "Test@12345", role = 3
        });

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Login_with_wrong_password_returns_401()
    {
        var client = _factory.CreateClient();
        var (_, email) = await client.RegisterAsync(role: 3, prefix: "badpw");

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email, password = "Wrong@12345"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Seeded_admin_can_login()
    {
        var client = _factory.CreateClient();

        var token = await client.LoginAsync("admin@findly.local", "Admin@123!");

        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task Protected_endpoint_without_token_returns_401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/listings", new { });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Buyer_cannot_access_admin_endpoint()
    {
        var client = _factory.CreateClient();
        var (token, _) = await client.RegisterAsync(role: 3, prefix: "noadmin");

        var response = await client.WithToken(token).GetAsync("/api/v1/admin/leads");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
