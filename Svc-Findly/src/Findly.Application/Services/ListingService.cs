using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class ListingService : IListingService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ListingService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ListingResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _uow.Listings.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<IEnumerable<ListingResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var listings = await _uow.Listings.GetAllAsync(page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<ListingResponse>>(listings);
    }

    public async Task<ListingResponse> CreateAsync(CreateListingRequest request, CancellationToken cancellationToken = default)
    {
        var listing = Listing.Create(
            request.Name,
            request.Slug,
            request.ShortDescription,
            request.WebsiteUrl,
            request.VendorId,
            request.PricingType,
            request.CreatedBy,
            request.Tagline,
            request.Description,
            request.LogoUrl,
            request.DemoUrl,
            request.FoundedYear,
            request.StartingPrice,
            request.PricePerUser,
            request.HasFreeTrial,
            request.FreeTrialDays);

        var created = await _uow.Listings.CreateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(created);
    }

    public async Task<ListingResponse> UpdateAsync(int id, UpdateListingRequest request, CancellationToken cancellationToken = default)
    {
        var listing = await _uow.Listings.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.UpdateDetails(
            request.Name,
            request.Slug,
            request.ShortDescription,
            request.WebsiteUrl,
            request.UpdatedBy,
            request.Tagline,
            request.Description,
            request.LogoUrl,
            request.DemoUrl,
            request.FoundedYear);

        listing.UpdatePricing(
            request.PricingType,
            request.UpdatedBy,
            request.StartingPrice,
            request.PricePerUser,
            request.HasFreeTrial,
            request.FreeTrialDays);

        await _uow.Listings.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Listings.DeleteAsync(id, cancellationToken);
    }

    public async Task<ListingResponse> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _uow.Listings.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Approve(updatedBy);
        await _uow.Listings.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _uow.Listings.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Reject(reason, updatedBy);
        await _uow.Listings.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> ArchiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _uow.Listings.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Archive(updatedBy);
        await _uow.Listings.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _uow.Listings.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Restore(updatedBy);
        await _uow.Listings.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }
}
