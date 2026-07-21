using Findly.Domain.Enums;

namespace Findly.Contracts.Auth.Responses;

public class AuthResponse
{
    public string   Token        { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public int      UserId       { get; set; }
    public string   Email        { get; set; }
    public string   FirstName    { get; set; }
    public string   LastName     { get; set; }
    public UserRole Role         { get; set; }
    public int?     VendorId     { get; set; }
}
