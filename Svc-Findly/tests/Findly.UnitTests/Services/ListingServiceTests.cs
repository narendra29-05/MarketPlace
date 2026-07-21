using AutoMapper;
using Findly.Application;
using Findly.Application.Common.Interfaces;
using Findly.Application.Listings.Services;
using Findly.Contracts.Listing.Requests;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace Findly.UnitTests.Services;

public class ListingServiceTests
{
    private readonly IListingRepository  _listingRepository  = Substitute.For<IListingRepository>();
    private readonly IVendorRepository   _vendorRepository   = Substitute.For<IVendorRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUserRepository     _userRepository     = Substitute.For<IUserRepository>();
    private readonly ICurrentUser        _currentUser        = Substitute.For<ICurrentUser>();
    private readonly ListingService      _service;

    public ListingServiceTests()
    {
        var mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(ApplicationModule).Assembly),
            NullLoggerFactory.Instance).CreateMapper();

        _currentUser.UserId.Returns(5);
        _currentUser.VendorId.Returns(1);
        _currentUser.IsAdmin.Returns(false);
        _currentUser.AuditName.Returns("vera@acme.io");

        _service = new ListingService(
            _listingRepository, _vendorRepository, _categoryRepository, _userRepository, _currentUser, mapper);
    }

    private static Vendor NewVendor(bool verified)
    {
        var vendor = Vendor.Create(
            "Vera", "Vendor", "vera@acme.io", "+14155550101", "Acme", 50, "t",
            Industry.ITAndSoftwareServices, "1 Main St", "", "Austin", "TX", "USA", "73301");
        if (verified) vendor.Verify("admin");
        return vendor;
    }

    private static CreateListingRequest Request() => new()
    {
        Name             = "AcmeCRM",
        Slug             = "acme-crm",
        ShortDescription = "CRM",
        WebsiteUrl       = "https://acme.io",
        PricingType      = PricingType.Paid,
        CategoryIds      = [1]
    };

    [Fact]
    public async Task Create_requires_verified_vendor()
    {
        _vendorRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(NewVendor(verified: false));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(Request()));
    }

    [Fact]
    public async Task Create_without_vendor_profile_throws()
    {
        _currentUser.VendorId.Returns((int?)null);
        var buyer = User.Create("Bob", "Buyer", "bob@corp.io", "hash", UserRole.Buyer, "t");
        _userRepository.GetByIdAsync(5, Arg.Any<CancellationToken>()).Returns(buyer);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(Request()));
    }

    [Fact]
    public async Task Create_rejects_unknown_category_ids()
    {
        _vendorRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(NewVendor(verified: true));
        _categoryRepository.GetByIdsAsync(Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(Request()));
    }

    [Fact]
    public async Task Update_of_another_vendors_listing_throws()
    {
        var otherVendorsListing = Listing.Create(
            "Other", "other", "d", "https://o.io", vendorId: 99, PricingType.Free, "t",
            null, null, null, null, null, null, null, false, null);
        _listingRepository.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(otherVendorsListing);

        var update = new UpdateListingRequest
        {
            Name = "X", Slug = "x", ShortDescription = "d", WebsiteUrl = "https://x.io",
            PricingType = PricingType.Free, CategoryIds = [1]
        };

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateAsync(3, update));
    }

    [Fact]
    public async Task Admin_bypasses_ownership_on_archive()
    {
        _currentUser.IsAdmin.Returns(true);
        var listing = Listing.Create(
            "Other", "other", "d", "https://o.io", vendorId: 99, PricingType.Free, "t",
            null, null, null, null, null, null, null, false, null);
        listing.Approve("admin");
        _listingRepository.GetByIdAsync(3, Arg.Any<CancellationToken>()).Returns(listing);

        var response = await _service.ArchiveAsync(3);

        Assert.Equal(ListingStatus.Archived, response.Status);
    }

    [Fact]
    public async Task Delete_of_missing_listing_throws_not_found()
    {
        _listingRepository.DeleteAsync(99, Arg.Any<CancellationToken>()).Returns(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteAsync(99));
    }
}
