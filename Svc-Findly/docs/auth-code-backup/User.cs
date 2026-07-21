using Findly.Domain.Enums;
using Findly.Domain.ValueObjects;

namespace Findly.Domain.Entities;

public class User : Entity
{
    protected User() { }

    public string   FirstName    { get; private set; }
    public string   LastName     { get; private set; }
    public Email    EmailAddress { get; private set; }
    public string   PasswordHash { get; private set; }
    public UserRole Role         { get; private set; }
    public int?     VendorId     { get; private set; }
    public bool     IsActive     { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static User Create(string firstName, string lastName, string email, string passwordHash, UserRole role, string createdBy)
    {
        return new User
        {
            FirstName    = firstName,
            LastName     = lastName,
            EmailAddress = Email.Create(email),
            PasswordHash = passwordHash,
            Role         = role,
            IsActive     = true,
            CreatedBy    = createdBy,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void LinkVendor(int vendorId, string updatedBy)
    {
        if (Role != UserRole.Vendor)
            throw new InvalidOperationException("Only users with the Vendor role can be linked to a vendor profile.");

        if (VendorId.HasValue)
            throw new InvalidOperationException("User is already linked to a vendor profile.");

        VendorId  = vendorId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string updatedBy)
    {
        IsActive  = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
