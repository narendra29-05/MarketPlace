using Findly.Domain.Enums;

namespace Findly.Contracts.Auth.Requests;

public class RegisterRequest
{
    public string   FirstName { get; set; }
    public string   LastName  { get; set; }
    public string   Email     { get; set; }
    public string   Password  { get; set; }
    public UserRole Role      { get; set; }
}
