using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ListingFeatureRepository : IListingFeatureRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ListingFeatureRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ListingFeature> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbListingFeature>(
            new CommandDefinition("SELECT * FROM fin.ListingFeatures WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<ListingFeature>(db);
    }

    public async Task<IEnumerable<ListingFeature>> GetByListingAsync(int listingId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM fin.ListingFeatures WHERE ListingId = @ListingId AND IsDeleted = 0 ORDER BY IsHighlighted DESC, Id;";
        var dbs = await _context.Connection.QueryAsync<DbListingFeature>(
            new CommandDefinition(sql, new { ListingId = listingId }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<ListingFeature>>(dbs);
    }

    public async Task<ListingFeature> CreateAsync(ListingFeature feature, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListingFeature>(feature);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<ListingFeature>(db);
    }

    public async Task<ListingFeature> UpdateAsync(ListingFeature feature, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListingFeature>(feature);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return feature;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.ListingFeatures SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
