using Findly.Domain.Enums;

namespace Findly.Contracts.Responses;

public class UserResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public Industry? IndustryType { get; set; }
    public UserRole Role { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
