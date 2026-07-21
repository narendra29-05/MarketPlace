using Findly.Domain.Enums;

namespace Findly.Contracts.Lead.Requests;

public class UpdateLeadStatusRequest
{
    public LeadStatus Status { get; set; }
}
