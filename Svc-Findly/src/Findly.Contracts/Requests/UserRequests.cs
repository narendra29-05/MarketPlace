using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

public class CreateUserRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public UserRole Role { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public Industry? IndustryType { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class UpdateUserRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public Industry? IndustryType { get; set; }
    public string UpdatedBy { get; set; } = null!;
}

public class ChangePasswordRequest
{
    public string Password { get; set; } = null!;
    public string UpdatedBy { get; set; } = null!;
}
