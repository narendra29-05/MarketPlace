using AutoMapper;
using Findly.Application.Common.Exceptions;
using Findly.Application.Common.Interfaces;
using Findly.Application.Common.Mappings;
using Findly.Application.Listings.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Category.Responses;
using Findly.Contracts.Listing.Requests;
using Findly.Contracts.Listing.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;

namespace Findly.Application.Listings.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _repository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public ListingService(
        IListingRepository repository,
        IVendorRepository vendorRepository,
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repository = repository;
        _vendorRepository = vendorRepository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ListingResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await GetListingOrThrowAsync(id, cancellationToken);

        var response = _mapper.Map<ListingResponse>(listing);
        response.Categories = _mapper.Map<List<CategoryResponse>>(
            await _categoryRepository.GetByListingIdAsync(id, cancellationToken));

        return response;
    }

    public async Task<PagedResponse<ListingResponse>> GetAllAsync(int page, int pageSize, ListingStatus? status = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _repository.GetAllAsync(page, pageSize, status, cancellationToken);
        var response = result.ToPagedResponse<Listing, ListingResponse>(_mapper);

        await HydrateCategoriesAsync(response.Items, cancellationToken);
        return response;
    }

    public async Task<PagedResponse<ListingResponse>> GetMyListingsAsync(int page, int pageSize, ListingStatus? status = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var vendorId = await RequireVendorIdAsync(cancellationToken);

        var result = await _repository.GetByVendorIdAsync(vendorId, status, page, pageSize, cancellationToken);
        var response = result.ToPagedResponse<Listing, ListingResponse>(_mapper);

        await HydrateCategoriesAsync(response.Items, cancellationToken);
        return response;
    }

    public async Task<ListingResponse> CreateAsync(CreateListingRequest request, CancellationToken cancellationToken = default)
    {
        var vendorId = await RequireVendorIdAsync(cancellationToken);

        var vendor = await _vendorRepository.GetByIdAsync(vendorId, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {vendorId} not found.");

        if (vendor.Status != VendorStatus.Verified)
            throw new InvalidOperationException("Vendor must be verified before creating listings.");

        await ValidateCategoriesAsync(request.CategoryIds, cancellationToken);

        var listing = Listing.Create(
            request.Name,
            request.Slug,
            request.ShortDescription,
            request.WebsiteUrl,
            vendorId,
            request.PricingType,
            _currentUser.AuditName,
            request.Tagline,
            request.Description,
            request.LogoUrl,
            request.DemoUrl,
            request.FoundedYear,
            request.StartingPrice,
            request.PricePerUser,
            request.HasFreeTrial,
            request.FreeTrialDays);

        var createdListing = await _repository.CreateAsync(listing, cancellationToken);
        await _repository.ReplaceCategoriesAsync(createdListing.Id, request.CategoryIds, cancellationToken);

        var response = _mapper.Map<ListingResponse>(createdListing);
        response.Categories = _mapper.Map<List<CategoryResponse>>(
            await _categoryRepository.GetByListingIdAsync(createdListing.Id, cancellationToken));

        return response;
    }

    public async Task<ListingResponse> UpdateAsync(int id, UpdateListingRequest request, CancellationToken cancellationToken = default)
    {
        var listing = await GetListingOrThrowAsync(id, cancellationToken);
        await EnsureOwnershipAsync(listing, cancellationToken);
        await ValidateCategoriesAsync(request.CategoryIds, cancellationToken);

        listing.UpdateDetails(
            request.Name,
            request.Slug,
            request.ShortDescription,
            request.WebsiteUrl,
            _currentUser.AuditName,
            request.Tagline,
            request.Description,
            request.LogoUrl,
            request.DemoUrl,
            request.FoundedYear);

        listing.UpdatePricing(
            request.PricingType,
            _currentUser.AuditName,
            request.StartingPrice,
            request.PricePerUser,
            request.HasFreeTrial,
            request.FreeTrialDays);

        await _repository.UpdateAsync(listing, cancellationToken);
        await _repository.ReplaceCategoriesAsync(id, request.CategoryIds, cancellationToken);

        var response = _mapper.Map<ListingResponse>(listing);
        response.Categories = _mapper.Map<List<CategoryResponse>>(
            await _categoryRepository.GetByListingIdAsync(id, cancellationToken));

        return response;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Listing with id {id} not found.");
    }

    public async Task<ListingResponse> ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await GetListingOrThrowAsync(id, cancellationToken);

        listing.Approve(_currentUser.AuditName);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var listing = await GetListingOrThrowAsync(id, cancellationToken);

        listing.Reject(reason, _currentUser.AuditName);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> ArchiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await GetListingOrThrowAsync(id, cancellationToken);
        await EnsureOwnershipAsync(listing, cancellationToken);

        listing.Archive(_currentUser.AuditName);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await GetListingOrThrowAsync(id, cancellationToken);
        await EnsureOwnershipAsync(listing, cancellationToken);

        listing.Restore(_currentUser.AuditName);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private async Task<Listing> GetListingOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var listing = await _repository.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        return listing;
    }

    private async Task<int> RequireVendorIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUser.VendorId.HasValue)
            return _currentUser.VendorId.Value;

        var userId = _currentUser.UserId
            ?? throw new AuthenticationFailedException("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.VendorId
            ?? throw new InvalidOperationException("Current user has no vendor profile. Complete vendor onboarding first.");
    }

    private async Task EnsureOwnershipAsync(Listing listing, CancellationToken cancellationToken)
    {
        if (_currentUser.IsAdmin)
            return;

        var vendorId = await RequireVendorIdAsync(cancellationToken);
        if (listing.VendorId != vendorId)
            throw new UnauthorizedAccessException("You can only manage your own listings.");
    }

    private async Task ValidateCategoriesAsync(IReadOnlyCollection<int> categoryIds, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        if (categories.Count != categoryIds.Count)
            throw new ArgumentException("One or more category ids are invalid.");
    }

    private async Task HydrateCategoriesAsync(IReadOnlyList<ListingResponse> items, CancellationToken cancellationToken)
    {
        if (items.Count == 0)
            return;

        var links = await _categoryRepository.GetByListingIdsAsync(
            items.Select(i => i.Id).ToList(), cancellationToken);

        var byListing = links
            .GroupBy(l => l.ListingId)
            .ToDictionary(g => g.Key, g => _mapper.Map<List<CategoryResponse>>(g.Select(l => l.Category).ToList()));

        foreach (var item in items)
        {
            if (byListing.TryGetValue(item.Id, out var categories))
                item.Categories = categories;
        }
    }
}
