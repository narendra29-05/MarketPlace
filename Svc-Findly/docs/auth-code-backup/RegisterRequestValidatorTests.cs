using Findly.Contracts.Auth.Requests;
using Findly.Contracts.Auth.Validators;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Validators;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    private static RegisterRequest ValidRequest(UserRole role = UserRole.Buyer) => new()
    {
        FirstName = "Bob",
        LastName  = "Buyer",
        Email     = "bob@corp.io",
        Password  = "Buyer@1234",
        Role      = role
    };

    [Theory]
    [InlineData(UserRole.Buyer)]
    [InlineData(UserRole.Vendor)]
    public void Buyer_and_vendor_can_register(UserRole role)
    {
        Assert.True(_validator.Validate(ValidRequest(role)).IsValid);
    }

    [Fact]
    public void Admin_role_is_blocked()
    {
        var result = _validator.Validate(ValidRequest(UserRole.Admin));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Role));
    }

    [Theory]
    [InlineData("short1A")]      // too short
    [InlineData("alllowercase1")] // no uppercase
    [InlineData("ALLUPPERCASE1")] // no lowercase
    [InlineData("NoDigitsHere!")] // no digit
    public void Weak_passwords_are_rejected(string password)
    {
        var request = ValidRequest();
        request.Password = password;

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Invalid_email_is_rejected()
    {
        var request = ValidRequest();
        request.Email = "not-an-email";

        Assert.False(_validator.Validate(request).IsValid);
    }
}
