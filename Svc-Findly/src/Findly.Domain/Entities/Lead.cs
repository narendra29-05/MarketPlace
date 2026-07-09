using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

// A buyer inquiry (demo/quote/contact request) captured against a Listing
// and routed to its Vendor. Tracks a simple sales pipeline via Status.
public class Lead : Entity
{
    protected Lead() { }

    // Relations
    public int ListingId { get; private set; }
    public int VendorId { get; private set; }
    public int? UserId { get; private set; } // null when submitted by a guest

    // Classification
    public LeadType Type { get; private set; }
    public LeadStatus Status { get; private set; }

    // Contact details (captured on the form)
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? Phone { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Message { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Lead Create(
        int listingId,
        int vendorId,
        LeadType type,
        string name,
        string email,
        string createdBy,
        int? userId = null,
        string? phone = null,
        string? companyName = null,
        string? message = null)
    {
        return new Lead
        {
            ListingId = listingId,
            VendorId = vendorId,
            UserId = userId,
            Type = type,
            Status = LeadStatus.New,
            Name = name,
            Email = email,
            Phone = phone,
            CompanyName = companyName,
            Message = message,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateStatus(LeadStatus status, string updatedBy)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
