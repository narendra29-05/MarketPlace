namespace Findly.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IVendorRepository Vendors { get; }
    IListingRepository Listings { get; }
    IUserRepository Users { get; }
    ICategoryRepository Categories { get; }
    ITagRepository Tags { get; }
    IListingCategoryRepository ListingCategories { get; }
    IListingTagRepository ListingTags { get; }
    IListingFeatureRepository ListingFeatures { get; }
    IListingMediaRepository ListingMedia { get; }
    IReviewRepository Reviews { get; }
    IReviewVoteRepository ReviewVotes { get; }
    ILeadRepository Leads { get; }
    IBookmarkRepository Bookmarks { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
