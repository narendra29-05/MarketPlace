using System.Data.SqlTypes;
using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
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

    public async Task<Listing> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbListing>(
            new CommandDefinition("SELECT * FROM fin.Listings WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));

        return _mapper.Map<Listing>(db);
    }

    public async Task<IEnumerable<Listing>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var offset = (page - 1) * pageSize;

        const string sql = """
            SELECT *
            FROM fin.Listings
            WHERE IsDeleted = 0
            ORDER BY Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
            """;

        var dbs = await _context.Connection.QueryAsync<DbListing>(
            new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));

        return _mapper.Map<IEnumerable<Listing>>(dbs);
    }

    public async Task<Listing> CreateAsync(Listing listing, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListing>(listing);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<Listing>(db);
    }

    public async Task<Listing> UpdateAsync(Listing listing, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListing>(listing);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return listing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.Listings SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));

        return rows > 0;
    }
}
