using Findly.Domain.ValueObjects;

namespace Findly.UnitTests.Domain;

public class ValueObjectTests
{
    [Fact]
    public void Email_is_trimmed_and_lowercased()
    {
        var email = Email.Create("  Bob@Corp.IO ");

        Assert.Equal("bob@corp.io", email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-an-email")]
    public void Email_rejects_invalid_input(string value)
    {
        Assert.Throws<ArgumentException>(() => Email.Create(value));
    }

    [Fact]
    public void Phone_keeps_valid_number()
    {
        var phone = PhoneNumber.Create("+14155550101");

        Assert.Equal("+14155550101", phone.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("12345678901234567890")]
    public void Phone_rejects_invalid_input(string value)
    {
        Assert.Throws<ArgumentException>(() => PhoneNumber.Create(value));
    }
}
