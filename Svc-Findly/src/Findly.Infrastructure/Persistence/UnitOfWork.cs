using Autofac;
using Findly.Domain.Repositories;

namespace Findly.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ILifetimeScope _scope;
    private readonly DatabaseContext _context;
    private bool _disposed;

    public UnitOfWork(ILifetimeScope scope, DatabaseContext context)
    {
        _scope = scope;
        _context = context;
    }

    public IVendorRepository Vendors => _scope.Resolve<IVendorRepository>();
    public IListingRepository Listings => _scope.Resolve<IListingRepository>();
    public IUserRepository Users => _scope.Resolve<IUserRepository>();
    public ICategoryRepository Categories => _scope.Resolve<ICategoryRepository>();
    public ITagRepository Tags => _scope.Resolve<ITagRepository>();
    public IListingCategoryRepository ListingCategories => _scope.Resolve<IListingCategoryRepository>();
    public IListingTagRepository ListingTags => _scope.Resolve<IListingTagRepository>();
    public IListingFeatureRepository ListingFeatures => _scope.Resolve<IListingFeatureRepository>();
    public IListingMediaRepository ListingMedia => _scope.Resolve<IListingMediaRepository>();
    public IReviewRepository Reviews => _scope.Resolve<IReviewRepository>();
    public IReviewVoteRepository ReviewVotes => _scope.Resolve<IReviewVoteRepository>();
    public ILeadRepository Leads => _scope.Resolve<ILeadRepository>();
    public IBookmarkRepository Bookmarks => _scope.Resolve<IBookmarkRepository>();

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _context.BeginTransactionAsync(cancellationToken);

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => _context.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default)
        => _context.RollbackAsync(cancellationToken);

    public void Dispose()
    {
        if (_disposed) return;
        _context.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        await _context.DisposeAsync();
        _disposed = true;
    }
}
