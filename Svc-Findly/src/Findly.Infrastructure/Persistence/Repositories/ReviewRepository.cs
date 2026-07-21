using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ReviewRepository : IReviewRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper         _mapper;

    public ReviewRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<Review?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbReview>(
            "SELECT * FROM [fin].[Reviews] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Review>(db);
    }

    public async Task<Review?> GetByListingAndEmailAsync(int listingId, string reviewerEmail, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbReview>(
            "SELECT * FROM [fin].[Reviews] WHERE ListingId = @ListingId AND ReviewerEmail = @ReviewerEmail",
            new { ListingId = listingId, ReviewerEmail = reviewerEmail.Trim().ToLowerInvariant() },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Review>(db);
    }

    public async Task<PagedResult<Review>> GetByListingAsync(int listingId, ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Reviews]
            WHERE ListingId = @ListingId AND (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Reviews]
            WHERE ListingId = @ListingId AND (@Status IS NULL OR [Status] = @Status)
            ORDER BY [CreatedAt] DESC
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        return await QueryPagedAsync(sql, new
        {
            ListingId = listingId,
            Status    = (int?)status,
            Page      = page,
            PageSize  = pageSize
        }, page, pageSize);
    }

    public async Task<PagedResult<Review>> GetAllAsync(ReviewStatus? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Reviews]
            WHERE (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Reviews]
            WHERE (@Status IS NULL OR [Status] = @Status)
            ORDER BY [CreatedAt] DESC
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        return await QueryPagedAsync(sql, new
        {
            Status   = (int?)status,
            Page     = page,
            PageSize = pageSize
        }, page, pageSize);
    }

    public async Task<Review> CreateAsync(Review review, CancellationToken cancellationToken)
    {
        var db    = _mapper.Map<DbReview>(review);
        var newId = await _context.Connection.InsertAsync(db, _context.CurrentTransaction);
        db.Id     = (int)newId;
        return _mapper.Map<Review>(db);
    }

    public async Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbReview>(review);
        await _context.Connection.UpdateAsync(db, _context.CurrentTransaction);
        return review;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            "DELETE FROM [fin].[Reviews] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return rows > 0;
    }

    public async Task<ListingRatingAggregate> GetApprovedAggregateAsync(int listingId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ISNULL(CAST(AVG(CAST(OverallRating         AS DECIMAL(9,4))) AS DECIMAL(3,2)), 0) AS AverageRating,
                ISNULL(CAST(AVG(CAST(FeaturesRating        AS DECIMAL(9,4))) AS DECIMAL(3,2)), 0) AS FeaturesRating,
                ISNULL(CAST(AVG(CAST(ValueForMoneyRating   AS DECIMAL(9,4))) AS DECIMAL(3,2)), 0) AS ValueForMoneyRating,
                ISNULL(CAST(AVG(CAST(CustomerSupportRating AS DECIMAL(9,4))) AS DECIMAL(3,2)), 0) AS CustomerSupportRating,
                COUNT(*) AS ReviewCount
            FROM [fin].[Reviews]
            WHERE ListingId = @ListingId AND [Status] = @Approved;
            """;

        return await _context.Connection.QuerySingleAsync<ListingRatingAggregate>(
            sql,
            new { ListingId = listingId, Approved = (int)ReviewStatus.Approved },
            _context.CurrentTransaction);
    }

    private async Task<PagedResult<Review>> QueryPagedAsync(string sql, object parameters, int page, int pageSize)
    {
        using var multi = await _context.Connection.QueryMultipleAsync(sql, parameters, _context.CurrentTransaction);

        var totalCount = await multi.ReadSingleAsync<int>();
        var dbs        = await multi.ReadAsync<DbReview>();

        return new PagedResult<Review>(_mapper.Map<List<Review>>(dbs), totalCount, page, pageSize);
    }
}
