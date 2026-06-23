using Findly.Domain.Enums;

namespace Findly.Contracts.Vendor.Requests;

public class CreateVendorRequest
{
    public string    FirstName    { get; set; }
    public string    LastName     { get; set; }
    public string    Email        { get; set; }
    public string    Phone        { get; set; }
    public string    CompanyName  { get; set; }
    public int?      CompanySize  { get; set; }
    public Industry Industry     { get; set; }
    public string    AddressLine1 { get; set; }
    public string?   AddressLine2 { get; set; }
    public string    City         { get; set; }
    public string    State        { get; set; }
    public string    Country      { get; set; }
    public string    PostalCode   { get; set; }
}
