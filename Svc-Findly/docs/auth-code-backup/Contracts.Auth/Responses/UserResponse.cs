using Findly.Domain.Enums;

namespace Findly.Contracts.Auth.Responses;

public class UserResponse
{
    public int      Id        { get; set; }
    public string   Email     { get; set; }
    public string   FirstName { get; set; }
    public string   LastName  { get; set; }
    public UserRole Role      { get; set; }
    public int?     VendorId  { get; set; }
    public bool     IsActive  { get; set; }
}
