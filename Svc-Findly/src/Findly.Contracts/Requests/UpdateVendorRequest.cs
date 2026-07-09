using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

public class UpdateVendorRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public int? CompanySize { get; set; }
    public Industry Industry { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string UpdatedBy { get; set; } = null!;
}
