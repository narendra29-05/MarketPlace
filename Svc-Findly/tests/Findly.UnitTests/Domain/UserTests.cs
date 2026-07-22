using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Domain;

public class UserTests
{
    [Fact]
    public void Create_is_active_with_normalized_email()
    {
        var user = User.Create("Vera", "Vendor", "  VERA@Acme.IO ", "hash", UserRole.Vendor, "test");

        Assert.True(user.IsActive);
        Assert.Equal("vera@acme.io", user.EmailAddress.Value);
    }

    [Theory]
    [InlineData(UserRole.Vendor)]
    [InlineData(UserRole.Admin)]
    public void LinkVendor_sets_vendor_id(UserRole role)
    {
        var user = User.Create("Vera", "Vendor", "vera@acme.io", "hash", role, "test");

        user.LinkVendor(42, "vera@acme.io");

        Assert.Equal(42, user.VendorId);
    }

    [Fact]
    public void LinkVendor_twice_throws()
    {
        var user = User.Create("Vera", "Vendor", "vera@acme.io", "hash", UserRole.Vendor, "test");
        user.LinkVendor(42, "vera@acme.io");

        Assert.Throws<InvalidOperationException>(() => user.LinkVendor(43, "vera@acme.io"));
    }

    [Fact]
    public void Deactivate_clears_active_flag()
    {
        var user = User.Create("X", "Y", "x@y.io", "hash", UserRole.Buyer, "test");

        user.Deactivate("admin");

        Assert.False(user.IsActive);
    }
}
