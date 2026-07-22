using Findly.Domain.Enums;

namespace Findly.Contracts.Lead.Requests;

public class CreateLeadRequest
{
    public LeadType LeadType { get; set; }
    public string FullName { get; set; }
    public string BusinessEmail { get; set; }
    public string? Phone { get; set; }
    public string? Company { get; set; }
    public int? CompanySize { get; set; }
    public string? Message { get; set; }
}
