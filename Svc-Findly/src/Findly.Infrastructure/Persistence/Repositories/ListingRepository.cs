using System.Text;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ListingRepository : IListingRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ListingRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Listing?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbListing>(
            "SELECT * FROM [fin].[Listings] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Listing>(db);
    }

    public async Task<Listing?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbListing>(
            "SELECT * FROM [fin].[Listings] WHERE Slug = @Slug",
            new { Slug = slug },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Listing>(db);
    }

    public async Task<IReadOnlyList<Listing>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
            return [];

        var dbs = await _context.Connection.QueryAsync<DbListing>(
            "SELECT * FROM [fin].[Listings] WHERE Id IN @Ids",
            new { Ids = ids },
            _context.CurrentTransaction);

        return _mapper.Map<List<Listing>>(dbs);
    }

    public async Task<PagedResult<Listing>> GetAllAsync(int page, int pageSize, ListingStatus? status, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Listings] WHERE (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Listings]
            WHERE (@Status IS NULL OR [Status] = @Status)
            ORDER BY [Id]
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        return await QueryPagedAsync(sql, new { Status = (int?)status, Page = page, PageSize = pageSize }, page, pageSize);
    }

    public async Task<PagedResult<Listing>> GetByVendorIdAsync(int vendorId, ListingStatus? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Listings]
            WHERE VendorId = @VendorId AND (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Listings]
            WHERE VendorId = @VendorId AND (@Status IS NULL OR [Status] = @Status)
            ORDER BY [Id]
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        return await QueryPagedAsync(sql, new
        {
            VendorId = vendorId,
            Status = (int?)status,
            Page = page,
            PageSize = pageSize
        }, page, pageSize);
    }

    public async Task<PagedResult<Listing>> SearchAsync(ListingSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var where = new StringBuilder("[Status] = @Published");

        if (!string.IsNullOrWhiteSpace(criteria.Search))
            where.Append("""
                 AND ([Name] LIKE @Search OR [Tagline] LIKE @Search
                      OR [ShortDescription] LIKE @Search OR [Description] LIKE @Search)
                """);

        if (criteria.CategoryId.HasValue || !string.IsNullOrWhiteSpace(criteria.CategorySlug))
            where.Append("""
                 AND EXISTS (
                    SELECT 1 FROM [fin].[ListingCategories] lc
                    JOIN [fin].[Categories] c ON c.Id = lc.CategoryId
                    WHERE lc.ListingId = l.Id
                      AND (@CategoryId IS NULL OR c.Id = @CategoryId)
                      AND (@CategorySlug IS NULL OR c.Slug = @CategorySlug))
                """);

        if (criteria.PricingType.HasValue)
            where.Append(" AND [PricingType] = @PricingType");

        if (criteria.MinRating.HasValue)
            where.Append(" AND [AverageRating] >= @MinRating");

        if (criteria.HasFreeTrial.HasValue)
            where.Append(" AND [HasFreeTrial] = @HasFreeTrial");

        // ORDER BY comes from a fixed switch — never from user-supplied strings.
        var orderBy = criteria.SortBy switch
        {
            ListingSortBy.ReviewCount => "[ReviewCount] DESC, [AverageRating] DESC",
            ListingSortBy.Newest => "[CreatedAt] DESC",
            ListingSortBy.Name => "[Name] ASC",
            _ => "[AverageRating] DESC, [ReviewCount] DESC"
        };

        var sql = $"""
            SELECT COUNT(*) FROM [fin].[Listings] l WHERE {where};

            SELECT * FROM [fin].[Listings] l
            WHERE {where}
            ORDER BY {orderBy}
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        var parameters = new
        {
            Published = (int)ListingStatus.Published,
            Search = $"%{criteria.Search?.Trim()}%",
            criteria.CategoryId,
            criteria.CategorySlug,
            PricingType = (int?)criteria.PricingType,
            criteria.MinRating,
            criteria.HasFreeTrial,
            criteria.Page,
            criteria.PageSize
        };

        return await QueryPagedAsync(sql, parameters, criteria.Page, criteria.PageSize);
    }

    public async Task<Listing> CreateAsync(Listing listing, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListing>(listing);
        var newId = await _context.Connection.InsertAsync(db, _context.CurrentTransaction);
        db.Id = (int)newId;
        return _mapper.Map<Listing>(db);
    }

    public async Task<Listing> UpdateAsync(Listing listing, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListing>(listing);
        await _context.Connection.UpdateAsync(db, _context.CurrentTransaction);
        return listing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            "DELETE FROM [fin].[Listings] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return rows > 0;
    }

    public async Task ReplaceCategoriesAsync(int listingId, IReadOnlyCollection<int> categoryIds, CancellationToken cancellationToken)
    {
        await _context.Connection.ExecuteAsync(
            "DELETE FROM [fin].[ListingCategories] WHERE ListingId = @ListingId",
            new { ListingId = listingId },
            _context.CurrentTransaction);

        if (categoryIds.Count == 0)
            return;

        await _context.Connection.ExecuteAsync(
            "INSERT INTO [fin].[ListingCategories] (ListingId, CategoryId) VALUES (@ListingId, @CategoryId)",
            categoryIds.Select(categoryId => new { ListingId = listingId, CategoryId = categoryId }),
            _context.CurrentTransaction);
    }

    private async Task<PagedResult<Listing>> QueryPagedAsync(string sql, object parameters, int page, int pageSize)
    {
        using var multi = await _context.Connection.QueryMultipleAsync(sql, parameters, _context.CurrentTransaction);

        var totalCount = await multi.ReadSingleAsync<int>();
        var dbs = await multi.ReadAsync<DbListing>();

        return new PagedResult<Listing>(_mapper.Map<List<Listing>>(dbs), totalCount, page, pageSize);
    }
}
