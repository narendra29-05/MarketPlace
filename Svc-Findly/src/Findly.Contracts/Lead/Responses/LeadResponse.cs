using Findly.Domain.Enums;

namespace Findly.Contracts.Lead.Responses;

public class LeadResponse
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public string? ListingName { get; set; }
    public int VendorId { get; set; }
    public LeadType LeadType { get; set; }
    public string FullName { get; set; }
    public string BusinessEmail { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public int? CompanySize { get; set; }
    public string? Message { get; set; }
    public LeadStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
