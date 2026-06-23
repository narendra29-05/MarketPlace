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
    private readonly IMapper         _mapper;

    public ListingRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<Listing> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbListing>(
            "SELECT * FROM Listings WHERE Id = @Id", new { Id = id });

        return _mapper.Map<Listing>(db);
    }

    public async Task<IEnumerable<Listing>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbs = await _context.Connection.QueryAsync<DbListing>(
            "SELECT * FROM Listings");

        return _mapper.Map<IEnumerable<Listing>>(dbs);
    }

    public async Task<Listing> CreateAync(Listing listing, CancellationToken cancellationToken)
    {
        var db    = _mapper.Map<DbListing>(listing);
        var newId = await _context.Connection.InsertAsync(db);
        db.Id     = (int)newId;
        return _mapper.Map<Listing>(db);
    }

    public async Task<Listing> UpdateAsync(Listing listing, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbListing>(listing);
        await _context.Connection.UpdateAsync(db);
        return listing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            "DELETE FROM Listings WHERE Id = @Id", new { Id = id });

        return rows > 0;
    }
}
