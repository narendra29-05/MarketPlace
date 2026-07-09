using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class ListingTaxonomyService : IListingTaxonomyService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public ListingTaxonomyService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // ---- Categories ----
    public async Task<IEnumerable<ListingCategoryResponse>> GetCategoriesAsync(int listingId, CancellationToken cancellationToken = default)
    {
        var links = await _uow.ListingCategories.GetByListingAsync(listingId, cancellationToken);
        return _mapper.Map<IEnumerable<ListingCategoryResponse>>(links);
    }

    public async Task<ListingCategoryResponse> AddCategoryAsync(int listingId, AddListingCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var link = ListingCategory.Create(listingId, request.CategoryId, request.CreatedBy);
        var created = await _uow.ListingCategories.CreateAsync(link, cancellationToken);
        return _mapper.Map<ListingCategoryResponse>(created);
    }

    public async Task RemoveCategoryAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.ListingCategories.DeleteAsync(id, cancellationToken);
    }

    // ---- Tags ----
    public async Task<IEnumerable<ListingTagResponse>> GetTagsAsync(int listingId, CancellationToken cancellationToken = default)
    {
        var links = await _uow.ListingTags.GetByListingAsync(listingId, cancellationToken);
        return _mapper.Map<IEnumerable<ListingTagResponse>>(links);
    }

    public async Task<ListingTagResponse> AddTagAsync(int listingId, AddListingTagRequest request, CancellationToken cancellationToken = default)
    {
        var link = ListingTag.Create(listingId, request.TagId, request.CreatedBy);
        var created = await _uow.ListingTags.CreateAsync(link, cancellationToken);
        return _mapper.Map<ListingTagResponse>(created);
    }

    public async Task RemoveTagAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.ListingTags.DeleteAsync(id, cancellationToken);
    }
}
