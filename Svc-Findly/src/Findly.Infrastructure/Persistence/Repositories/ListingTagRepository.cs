using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ListingTagRepository : IListingTagRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ListingTagRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ListingTag>> GetByListingAsync(int listingId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM fin.ListingTags WHERE ListingId = @ListingId AND IsDeleted = 0 ORDER BY Id;";
        var dbs = await _context.Connection.QueryAsync<DbListingTag>(
            new CommandDefinition(sql, new { ListingId = listingId }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<ListingTag>>(dbs);
    }

    public async Task<ListingTag> CreateAsync(ListingTag link, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListingTag>(link);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<ListingTag>(db);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.ListingTags SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
