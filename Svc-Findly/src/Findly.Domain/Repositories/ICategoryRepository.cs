using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public sealed record ListingCategoryLink(int ListingId, Category Category);

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<Category>> GetByListingIdAsync(int listingId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ListingCategoryLink>> GetByListingIdsAsync(IReadOnlyCollection<int> listingIds, CancellationToken cancellationToken);

    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken);

    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
