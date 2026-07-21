using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Findly.IntegrationTests;

/// <summary>
/// Boots the real API in-memory. Requires the SQL Server container (docker compose up -d)
/// and applied migrations — see scripts/run-integration-tests.sh.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>;

[CollectionDefinition("api")]
public class ApiCollection : ICollectionFixture<ApiFactory>;

public static class HttpExtensions
{
    public static async Task<JsonElement> ReadJsonAsync(this HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(body).RootElement.Clone();
    }

    public static HttpClient WithToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    /// <summary>Registers a fresh user (unique email) and returns (token, email).</summary>
    public static async Task<(string Token, string Email)> RegisterAsync(this HttpClient client, int role, string prefix)
    {
        var email    = $"{prefix}-{Guid.NewGuid():N}@test.findly.local";
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            firstName = "Test",
            lastName  = prefix,
            email,
            password  = "Test@12345",
            role
        });
        response.EnsureSuccessStatusCode();

        var json = await response.ReadJsonAsync();
        return (json.GetProperty("token").GetString()!, email);
    }

    public static async Task<string> LoginAsync(this HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        var json = await response.ReadJsonAsync();
        return json.GetProperty("token").GetString()!;
    }
}
