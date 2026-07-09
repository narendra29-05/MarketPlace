using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ListingCategoryRepository : IListingCategoryRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ListingCategoryRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListingCategory>> GetByListingAsync(int listingId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM fin.ListingCategories WHERE ListingId = @ListingId AND IsDeleted = 0 ORDER BY Id;";
        var dbs = await _context.Connection.QueryAsync<DbListingCategory>(
            new CommandDefinition(sql, new { ListingId = listingId }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<ListingCategory>>(dbs);
    }

    public async Task<ListingCategory> CreateAsync(ListingCategory link, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListingCategory>(link);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<ListingCategory>(db);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.ListingCategories SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
