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
}
