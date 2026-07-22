using Findly.Contracts.Catalog.Requests;
using Findly.Contracts.Catalog.Responses;
using Findly.Contracts.Category.Responses;
using Findly.Contracts.Common;
using Findly.Contracts.Review.Responses;

namespace Findly.Application.Catalog.Interfaces;

public interface ICatalogService
{
    Task<PagedResponse<ListingSummaryResponse>> SearchAsync(CatalogSearchRequest request, CancellationToken cancellationToken = default);
    Task<ListingDetailResponse> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<PagedResponse<ReviewResponse>> GetReviewsAsync(string slug, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CompareResponse> CompareAsync(IReadOnlyCollection<int> listingIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken = default);
}
