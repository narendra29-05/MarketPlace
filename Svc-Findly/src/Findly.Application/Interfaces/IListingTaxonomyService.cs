using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

// Manages a Listing's Category and Tag links.
public interface IListingTaxonomyService
{
    Task<IEnumerable<ListingCategoryResponse>> GetCategoriesAsync(int listingId, CancellationToken cancellationToken = default);
    Task<ListingCategoryResponse> AddCategoryAsync(int listingId, AddListingCategoryRequest request, CancellationToken cancellationToken = default);
    Task RemoveCategoryAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ListingTagResponse>> GetTagsAsync(int listingId, CancellationToken cancellationToken = default);
    Task<ListingTagResponse> AddTagAsync(int listingId, AddListingTagRequest request, CancellationToken cancellationToken = default);
    Task RemoveTagAsync(int id, CancellationToken cancellationToken = default);
}
