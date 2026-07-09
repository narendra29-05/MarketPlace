using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ListingMediaRepository : IListingMediaRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ListingMediaRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ListingMedia> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbListingMedia>(
            new CommandDefinition("SELECT * FROM fin.ListingMedia WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<ListingMedia>(db);
    }

    public async Task<IEnumerable<ListingMedia>> GetByListingAsync(int listingId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM fin.ListingMedia WHERE ListingId = @ListingId AND IsDeleted = 0 ORDER BY DisplayOrder, Id;";
        var dbs = await _context.Connection.QueryAsync<DbListingMedia>(
            new CommandDefinition(sql, new { ListingId = listingId }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<ListingMedia>>(dbs);
    }

    public async Task<ListingMedia> CreateAsync(ListingMedia media, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListingMedia>(media);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<ListingMedia>(db);
    }

    public async Task<ListingMedia> UpdateAsync(ListingMedia media, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListingMedia>(media);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return media;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.ListingMedia SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
