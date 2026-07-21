using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Infrastructure.Security;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Findly.UnitTests.Security;

public class JwtTokenGeneratorTests
{
    private static readonly JwtOptions Options = new()
    {
        Issuer        = "Findly",
        Audience      = "Findly.Api",
        SigningKey    = "unit-test-signing-key-needs-32-chars-min!!",
        ExpiryMinutes = 60
    };

    private readonly JwtTokenGenerator _generator = new(Options);

    [Fact]
    public void Generates_token_with_identity_claims()
    {
        var user = User.Create("Bob", "Buyer", "bob@corp.io", "hash", UserRole.Buyer, "test");

        var (token, expiresAtUtc) = _generator.Generate(user);
        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

        Assert.Equal("Findly", jwt.Issuer);
        Assert.Equal("bob@corp.io", jwt.GetClaim("email").Value);
        Assert.Equal("Bob Buyer", jwt.GetClaim("name").Value);
        Assert.Equal("Buyer", jwt.GetClaim("role").Value);
        Assert.DoesNotContain(jwt.Claims, c => c.Type == "vendor_id");
        Assert.InRange(expiresAtUtc, DateTime.UtcNow.AddMinutes(59), DateTime.UtcNow.AddMinutes(61));
    }

    [Fact]
    public void Includes_vendor_id_claim_after_vendor_link()
    {
        var user = User.Create("Vera", "Vendor", "vera@acme.io", "hash", UserRole.Vendor, "test");
        user.LinkVendor(42, "vera@acme.io");

        var (token, _) = _generator.Generate(user);
        var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

        Assert.Equal("42", jwt.GetClaim("vendor_id").Value);
        Assert.Equal("Vendor", jwt.GetClaim("role").Value);
    }
}
