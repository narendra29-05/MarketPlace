using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.UnitTests.Domain;

public class VendorTests
{
    private static Vendor NewVendor() => Vendor.Create(
        "Vera", "Vendor", "vera@acme.io", "+14155550101", "Acme", 50, "test",
        Industry.ITAndSoftwareServices, "1 Main St", "", "Austin", "TX", "USA", "73301");

    [Fact]
    public void Create_starts_pending()
    {
        var vendor = NewVendor();

        Assert.Equal(VendorStatus.Pending, vendor.Status);
        Assert.Equal("vera@acme.io", vendor.EmailAddress.Value);
        Assert.Null(vendor.RejectionReason);
    }

    [Fact]
    public void Verify_from_pending_sets_verified()
    {
        var vendor = NewVendor();

        vendor.Verify("admin");

        Assert.Equal(VendorStatus.Verified, vendor.Status);
        Assert.Null(vendor.RejectionReason);
        Assert.Equal("admin", vendor.UpdatedBy);
    }

    [Fact]
    public void Verify_twice_throws()
    {
        var vendor = NewVendor();
        vendor.Verify("admin");

        Assert.Throws<InvalidOperationException>(() => vendor.Verify("admin"));
    }

    [Fact]
    public void Reject_from_pending_stores_reason()
    {
        var vendor = NewVendor();

        vendor.Reject("Incomplete details", "admin");

        Assert.Equal(VendorStatus.Rejected, vendor.Status);
        Assert.Equal("Incomplete details", vendor.RejectionReason);
    }

    [Fact]
    public void Reject_after_verify_throws()
    {
        var vendor = NewVendor();
        vendor.Verify("admin");

        Assert.Throws<InvalidOperationException>(() => vendor.Reject("nope", "admin"));
    }
}
