using Findly.Domain.Enums;

namespace Findly.Contracts.Responses;

public class LeadResponse
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public int VendorId { get; set; }
    public int? UserId { get; set; }
    public LeadType Type { get; set; }
    public LeadStatus Status { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
    public string? Message { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
