using System.Security.Claims;
using Findly.Application.Common.Interfaces;
using Findly.Domain.Enums;

namespace Findly.Api.Auth;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public int? UserId => int.TryParse(Principal?.FindFirstValue("sub"), out var id) ? id : null;

    public string? Email => Principal?.FindFirstValue("email");

    public string? Name => Principal?.FindFirstValue("name");

    public string? Role => Principal?.FindFirstValue("role");

    public int? VendorId => int.TryParse(Principal?.FindFirstValue("vendor_id"), out var id) ? id : null;

    public bool IsAdmin => Role == nameof(UserRole.Admin);

    public string AuditName => Email ?? "anonymous";
}
