using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class ListingMediaService : IListingMediaService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ListingMediaService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListingMediaResponse>> GetByListingAsync(int listingId, CancellationToken cancellationToken = default)
    {
        var media = await _uow.ListingMedia.GetByListingAsync(listingId, cancellationToken);
        return _mapper.Map<IEnumerable<ListingMediaResponse>>(media);
    }

    public async Task<ListingMediaResponse> AddAsync(int listingId, AddListingMediaRequest request, CancellationToken cancellationToken = default)
    {
        var media = ListingMedia.Create(listingId, request.Type, request.Url, request.CreatedBy, request.Caption, request.DisplayOrder);
        var created = await _uow.ListingMedia.CreateAsync(media, cancellationToken);
        return _mapper.Map<ListingMediaResponse>(created);
    }

    public async Task<ListingMediaResponse> UpdateAsync(int id, UpdateListingMediaRequest request, CancellationToken cancellationToken = default)
    {
        var media = await _uow.ListingMedia.GetByIdAsync(id, cancellationToken);
        if (media is null)
            throw new KeyNotFoundException($"ListingMedia with id {id} not found.");

        media.Update(request.Url, request.Type, request.UpdatedBy, request.Caption, request.DisplayOrder);
        await _uow.ListingMedia.UpdateAsync(media, cancellationToken);
        return _mapper.Map<ListingMediaResponse>(media);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.ListingMedia.DeleteAsync(id, cancellationToken);
    }
}
