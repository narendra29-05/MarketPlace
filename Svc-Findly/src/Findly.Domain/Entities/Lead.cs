using Findly.Domain.Enums;
using Findly.Domain.ValueObjects;

namespace Findly.Domain.Entities;

public class Lead : Entity
{
    protected Lead() { }

    public int          ListingId     { get; private set; }
    public int          VendorId      { get; private set; }
    public LeadType     LeadType      { get; private set; }
    public string       FullName      { get; private set; }
    public Email        BusinessEmail { get; private set; }
    public PhoneNumber? Phone         { get; private set; }
    public string?      Company       { get; private set; }
    public int?         CompanySize   { get; private set; }
    public string?      Message       { get; private set; }
    public LeadStatus   Status        { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Lead Create(
        int      listingId,
        int      vendorId,
        LeadType leadType,
        string   fullName,
        string   businessEmail,
        string?  phone,
        string?  company,
        int?     companySize,
        string?  message,
        string   createdBy)
    {
        return new Lead
        {
            ListingId     = listingId,
            VendorId      = vendorId,
            LeadType      = leadType,
            FullName      = fullName,
            BusinessEmail = Email.Create(businessEmail),
            Phone         = string.IsNullOrWhiteSpace(phone) ? null : PhoneNumber.Create(phone),
            Company       = company,
            CompanySize   = companySize,
            Message       = message,
            Status        = LeadStatus.New,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void TransitionTo(LeadStatus newStatus, string updatedBy)
    {
        var allowed = Status switch
        {
            LeadStatus.New       => new[] { LeadStatus.Contacted, LeadStatus.Lost },
            LeadStatus.Contacted => new[] { LeadStatus.Qualified, LeadStatus.Lost },
            LeadStatus.Qualified => new[] { LeadStatus.Converted, LeadStatus.Lost },
            _                    => Array.Empty<LeadStatus>()
        };

        if (!allowed.Contains(newStatus))
            throw new InvalidOperationException($"Cannot transition lead from '{Status}' to '{newStatus}'.");

        Status    = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
