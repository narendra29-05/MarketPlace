using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

public class CreateVendorRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public int? CompanySize { get; set; }
    public Industry Industry { get; set; }
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string CreatedBy { get; set; } = null!;
}
