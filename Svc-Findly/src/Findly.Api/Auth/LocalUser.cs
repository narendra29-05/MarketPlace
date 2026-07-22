using Findly.Application.Common.Interfaces;
using Findly.Domain.Enums;

namespace Findly.Api.Auth;

/// <summary>
/// Fixed local-development identity — authentication is disabled, every request
/// runs as the seeded admin account (admin@findly.local, Users.Id = 1).
/// Vendor flows resolve the vendor link from that Users row after onboarding.
/// </summary>
public sealed class LocalUser : ICurrentUser
{
    public bool IsAuthenticated => true;

    public int? UserId => 1;

    public string? Email => "admin@findly.local";

    public string? Name => "Findly Admin";

    public string? Role => nameof(UserRole.Admin);

    public int? VendorId => null;

    public bool IsAdmin => true;

    public string AuditName => "admin@findly.local";
}
