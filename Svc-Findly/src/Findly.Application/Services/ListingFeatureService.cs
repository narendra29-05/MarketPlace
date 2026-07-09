using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class ListingFeatureService : IListingFeatureService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ListingFeatureService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListingFeatureResponse>> GetByListingAsync(int listingId, CancellationToken cancellationToken = default)
    {
        var features = await _uow.ListingFeatures.GetByListingAsync(listingId, cancellationToken);
        return _mapper.Map<IEnumerable<ListingFeatureResponse>>(features);
    }

    public async Task<ListingFeatureResponse> AddAsync(int listingId, AddListingFeatureRequest request, CancellationToken cancellationToken = default)
    {
        var feature = ListingFeature.Create(listingId, request.Name, request.CreatedBy, request.Description, request.IsHighlighted);
        var created = await _uow.ListingFeatures.CreateAsync(feature, cancellationToken);
        return _mapper.Map<ListingFeatureResponse>(created);
    }

    public async Task<ListingFeatureResponse> UpdateAsync(int id, UpdateListingFeatureRequest request, CancellationToken cancellationToken = default)
    {
        var feature = await _uow.ListingFeatures.GetByIdAsync(id, cancellationToken);
        if (feature is null)
            throw new KeyNotFoundException($"ListingFeature with id {id} not found.");

        feature.Update(request.Name, request.UpdatedBy, request.Description, request.IsHighlighted);
        await _uow.ListingFeatures.UpdateAsync(feature, cancellationToken);
        return _mapper.Map<ListingFeatureResponse>(feature);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.ListingFeatures.DeleteAsync(id, cancellationToken);
    }
}
