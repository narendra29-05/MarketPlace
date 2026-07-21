using AutoMapper;
using Findly.Application.Catalog.Interfaces;
using Findly.Application.Common.Mappings;
using Findly.Contracts.Catalog.Requests;
using Findly.Contracts.Catalog.Responses;
using Findly.Contracts.Category.Responses;
using Findly.Contracts.Common;
using Findly.Contracts.Review.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;

namespace Findly.Application.Catalog.Services;

public class CatalogService : ICatalogService
{
    private readonly IListingRepository  _listingRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IVendorRepository   _vendorRepository;
    private readonly IReviewRepository   _reviewRepository;
    private readonly IMapper             _mapper;

    public CatalogService(
        IListingRepository  listingRepository,
        ICategoryRepository categoryRepository,
        IVendorRepository   vendorRepository,
        IReviewRepository   reviewRepository,
        IMapper             mapper)
    {
        _listingRepository  = listingRepository;
        _categoryRepository = categoryRepository;
        _vendorRepository   = vendorRepository;
        _reviewRepository   = reviewRepository;
        _mapper             = mapper;
    }

    public async Task<PagedResponse<ListingSummaryResponse>> SearchAsync(CatalogSearchRequest request, CancellationToken cancellationToken = default)
    {
        var criteria = new ListingSearchCriteria(
            request.Search,
            request.CategoryId,
            request.CategorySlug,
            request.PricingType,
            request.MinRating,
            request.HasFreeTrial,
            request.SortBy,
            Math.Max(1, request.Page),
            Math.Clamp(request.PageSize, 1, 50));

        var result   = await _listingRepository.SearchAsync(criteria, cancellationToken);
        var response = result.ToPagedResponse<Listing, ListingSummaryResponse>(_mapper);

        var categoriesByListing = await GetCategoriesByListingAsync(
            response.Items.Select(i => i.Id).ToList(), cancellationToken);

        foreach (var item in response.Items)
        {
            if (categoriesByListing.TryGetValue(item.Id, out var categories))
                item.Categories = categories;
        }

        return response;
    }

    public async Task<ListingDetailResponse> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var listing = await GetPublishedListingBySlugAsync(slug, cancellationToken);

        var response = _mapper.Map<ListingDetailResponse>(listing);

        var vendor = await _vendorRepository.GetByIdAsync(listing.VendorId, cancellationToken);
        response.VendorCompanyName = vendor?.CompanyName;

        response.Categories = _mapper.Map<List<CategoryResponse>>(
            await _categoryRepository.GetByListingIdAsync(listing.Id, cancellationToken));

        return response;
    }

    public async Task<PagedResponse<ReviewResponse>> GetReviewsAsync(string slug, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var listing = await GetPublishedListingBySlugAsync(slug, cancellationToken);

        var result = await _reviewRepository.GetByListingAsync(
            listing.Id, ReviewStatus.Approved, page, pageSize, cancellationToken);

        return result.ToPagedResponse<Review, ReviewResponse>(_mapper);
    }

    public async Task<CompareResponse> CompareAsync(IReadOnlyCollection<int> listingIds, CancellationToken cancellationToken = default)
    {
        var distinctIds = listingIds.Distinct().ToList();

        if (distinctIds.Count is < 2 or > 4)
            throw new ArgumentException("Provide between 2 and 4 distinct listing ids to compare.");

        var listings = (await _listingRepository.GetByIdsAsync(distinctIds, cancellationToken))
            .Where(l => l.Status == ListingStatus.Published)
            .ToList();

        if (listings.Count != distinctIds.Count)
            throw new ArgumentException("All listings to compare must exist and be published.");

        var categoriesByListing = await GetCategoriesByListingAsync(distinctIds, cancellationToken);

        var items = listings
            .Select(listing =>
            {
                var item = _mapper.Map<CompareItemResponse>(listing);
                if (categoriesByListing.TryGetValue(listing.Id, out var categories))
                    item.Categories = categories;
                return item;
            })
            .ToList();

        return new CompareResponse { Listings = items };
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(includeInactive: false, cancellationToken);
        return _mapper.Map<List<CategoryResponse>>(categories);
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private async Task<Listing> GetPublishedListingBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var listing = await _listingRepository.GetBySlugAsync(slug, cancellationToken);
        if (listing is null || listing.Status != ListingStatus.Published)
            throw new KeyNotFoundException($"Listing '{slug}' not found.");

        return listing;
    }

    private async Task<Dictionary<int, List<CategoryResponse>>> GetCategoriesByListingAsync(
        IReadOnlyCollection<int> listingIds, CancellationToken cancellationToken)
    {
        if (listingIds.Count == 0)
            return [];

        var links = await _categoryRepository.GetByListingIdsAsync(listingIds, cancellationToken);

        return links
            .GroupBy(l => l.ListingId)
            .ToDictionary(g => g.Key, g => _mapper.Map<List<CategoryResponse>>(g.Select(l => l.Category).ToList()));
    }
}
