using AutoMapper;
using Findly.Application.Listings.Interfaces;
using Findly.Contracts.Listing.Requests;
using Findly.Contracts.Listing.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Listings.Services;

public class ListingService : IListingService
{
    private readonly IListingRepository _repository;
    private readonly IMapper            _mapper;

    public ListingService(IListingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper     = mapper;
    }

    public async Task<ListingResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var listing = await _repository.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<IEnumerable<ListingResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var listings = await _repository.GetAllAsync(cancellationToken);
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
            "system",
            request.Tagline,
            request.Description,
            request.LogoUrl,
            request.DemoUrl,
            request.FoundedYear,
            request.StartingPrice,
            request.PricePerUser,
            request.HasFreeTrial,
            request.FreeTrialDays);

        var createdListing = await _repository.CreateAync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(createdListing);
    }

    public async Task<ListingResponse> UpdateAsync(int id, UpdateListingRequest request, CancellationToken cancellationToken = default)
    {
        var listing = await _repository.GetByIdAsync(id, cancellationToken);
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

        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ListingResponse> ApproveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _repository.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Approve(updatedBy);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _repository.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Reject(reason, updatedBy);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }

    public async Task<ListingResponse> ArchiveAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var listing = await _repository.GetByIdAsync(id, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {id} not found.");

        listing.Archive(updatedBy);
        await _repository.UpdateAsync(listing, cancellationToken);
        return _mapper.Map<ListingResponse>(listing);
    }
}
