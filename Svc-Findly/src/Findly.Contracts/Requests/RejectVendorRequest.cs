namespace Findly.Contracts.Requests;

public class RejectVendorRequest
{
    public string Reason { get; set; } = null!;
    public string UpdatedBy { get; set; } = null!;
}
