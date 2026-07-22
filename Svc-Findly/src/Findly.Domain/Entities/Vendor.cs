using Findly.Domain.Enums;
using Findly.Domain.ValueObjects;

namespace Findly.Domain.Entities;

public class Vendor : Entity
{
    protected Vendor() { }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email EmailAddress { get; private set; }
    public PhoneNumber Phone { get; private set; }
    public Address Address { get; private set; }
    public string CompanyName { get; private set; }
    public int? CompanySize { get; private set; }
    public Industry IndustryType { get; private set; }
    public string WebsiteUrl { get; private set; }
    public string? LogoUrl { get; private set; }
    public VendorStatus Status { get; private set; }
    public string? RejectionReason { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Vendor Create(string firstName, string lastName, string email, string phone, string companyName, int? companySize, string createdBy, Industry industry,
        string addressLine1, string addressLine2, string city, string state, string country, string postalCode)
    {
        return new Vendor
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = Email.Create(email),
            Phone = PhoneNumber.Create(phone),
            CompanyName = companyName,
            CompanySize = companySize,
            IndustryType = industry,
            Status = VendorStatus.Pending,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Address = Address.Create(addressLine1, addressLine2, city, state, country, postalCode)
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateDetails(string firstName, string lastName, string email, string phone, string companyName,
        int? companySize, Industry industry, string? websiteUrl, string? logoUrl, string updatedBy)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = Email.Create(email);
        Phone = PhoneNumber.Create(phone);
        CompanyName = companyName;
        CompanySize = companySize;
        IndustryType = industry;
        WebsiteUrl = websiteUrl;
        LogoUrl = logoUrl;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateAddress(string? addressLine1, string? addressLine2, string? city, string? state, string? country, string? postalCode, string updatedBy)
    {
        Address = Address.Create(addressLine1, addressLine2, city, state, country, postalCode);
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }


    public void Verify(string updatedBy)
    {
        if (Status != VendorStatus.Pending)
            throw new InvalidOperationException("Only pending vendors can be verified.");

        Status = VendorStatus.Verified;
        RejectionReason = null;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Reject(string reason, string updatedBy)
    {
        if (Status != VendorStatus.Pending)
            throw new InvalidOperationException("Only pending vendors can be rejected.");

        Status = VendorStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

}
