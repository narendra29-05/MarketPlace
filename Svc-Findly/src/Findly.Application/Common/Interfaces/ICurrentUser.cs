namespace Findly.Application.Common.Interfaces;

public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    int? UserId { get; }
    string? Email { get; }
    string? Name { get; }
    string? Role { get; }
    int? VendorId { get; }
    bool IsAdmin { get; }

    /// <summary>Identity written to CreatedBy/UpdatedBy audit columns ("anonymous" when unauthenticated).</summary>
    string AuditName { get; }
}
