using System.Text;
using Findly.Application.Common.Interfaces;
using Findly.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Findly.Infrastructure.Security;

internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(JwtOptions options)
    {
        _options = options;
    }

    public (string Token, DateTime ExpiresAtUtc) Generate(User user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub]   = user.Id.ToString(),
            [JwtRegisteredClaimNames.Email] = user.EmailAddress.Value,
            [JwtRegisteredClaimNames.Name]  = $"{user.FirstName} {user.LastName}",
            [JwtRegisteredClaimNames.Jti]   = Guid.NewGuid().ToString(),
            ["role"]                        = user.Role.ToString()
        };

        if (user.VendorId.HasValue)
            claims["vendor_id"] = user.VendorId.Value.ToString();

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer             = _options.Issuer,
            Audience           = _options.Audience,
            Expires            = expiresAtUtc,
            Claims             = claims,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);
        return (token, expiresAtUtc);
    }
}
