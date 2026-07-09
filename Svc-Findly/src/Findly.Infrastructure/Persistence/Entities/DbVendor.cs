using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Vendors")]
internal class DbVendor
{
    [Key]
    public int          Id              { get; set; }
    public string       FirstName       { get; set; } = null!;
    public string       LastName        { get; set; } = null!;
    public string       Email           { get; set; } = null!;
    public string       Phone           { get; set; } = null!;
    public string       CompanyName     { get; set; } = null!;
    public int?         CompanySize     { get; set; }
    public Industry?    IndustryType    { get; set; }
    public string?      WebsiteUrl      { get; set; }
    public string?      LogoUrl         { get; set; }
    public VendorStatus Status         { get; set; }
    public string?      RejectionReason { get; set; }
    public string?      AddressLine1    { get; set; }
    public string?      AddressLine2    { get; set; }
    public string?      City            { get; set; }
    public string?      State           { get; set; }
    public string?      Country         { get; set; }
    public string?      PostalCode      { get; set; }
    public string?      CreatedBy       { get; set; }
    public DateTime     CreatedAt       { get; set; }
    public string?      UpdatedBy       { get; set; }
    public DateTime     UpdatedAt       { get; set; }
}
