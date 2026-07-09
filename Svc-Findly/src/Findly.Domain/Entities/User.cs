using Findly.Domain.Enums;
using Findly.Domain.ValueObjects;

namespace Findly.Domain.Entities;

public class User : Entity
{
    protected User() { }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email EmailAddress { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? AvatarUrl { get; private set; }
    public string? JobTitle { get; private set; }
    public string? CompanyName { get; private set; }
    public Industry? IndustryType { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsEmailVerified { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static User Create(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        UserRole role,
        string createdBy,
        string? jobTitle = null,
        string? companyName = null,
        Industry? industry = null)
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            EmailAddress = Email.Create(email),
            PasswordHash = passwordHash,
            Role = role,
            JobTitle = jobTitle,
            CompanyName = companyName,
            IndustryType = industry,
            IsEmailVerified = false,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateProfile(
        string firstName,
        string lastName,
        string updatedBy,
        string? avatarUrl,
        string? jobTitle,
        string? companyName,
        Industry? industry)
    {
        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
        JobTitle = jobTitle;
        CompanyName = companyName;
        IndustryType = industry;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void ChangePassword(string passwordHash, string updatedBy)
    {
        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void VerifyEmail(string updatedBy)
    {
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
