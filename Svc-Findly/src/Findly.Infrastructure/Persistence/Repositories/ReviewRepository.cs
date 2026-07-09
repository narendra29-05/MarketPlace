using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ReviewRepository : IReviewRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ReviewRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Review> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbReview>(
            new CommandDefinition("SELECT * FROM fin.Reviews WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<Review>(db);
    }

    public async Task<IEnumerable<Review>> GetByListingAsync(int listingId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        var offset = (page - 1) * pageSize;

        const string sql = "SELECT * FROM fin.Reviews WHERE ListingId = @ListingId AND IsDeleted = 0 ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        var dbs = await _context.Connection.QueryAsync<DbReview>(
            new CommandDefinition(sql, new { ListingId = listingId, Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<Review>>(dbs);
    }

    public async Task<IEnumerable<Review>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        var offset = (page - 1) * pageSize;

        const string sql = "SELECT * FROM fin.Reviews WHERE IsDeleted = 0 ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        var dbs = await _context.Connection.QueryAsync<DbReview>(
            new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<Review>>(dbs);
    }

    public async Task<Review> CreateAsync(Review review, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbReview>(review);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<Review>(db);
    }

    public async Task<Review> UpdateAsync(Review review, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbReview>(review);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return review;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.Reviews SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
