using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class VendorRepository : IVendorRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper         _mapper;

    public VendorRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper  = mapper;
    }

    public async Task<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbVendor>(
            "SELECT * FROM Vendors WHERE Id = @Id", new { Id = id });

        return _mapper.Map<Vendor>(db);
    }

    public async Task<IEnumerable<Vendor>> GetAllAsync(CancellationToken cancellationToken)
    {
        var dbs = await _context.Connection.QueryAsync<DbVendor>(
            "SELECT * FROM Vendors");

        return _mapper.Map<IEnumerable<Vendor>>(dbs);
    }

    public async Task<Vendor> CreateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        var db    = _mapper.Map<DbVendor>(vendor);
        var newId = await _context.Connection.InsertAsync(db);
        db.Id     = (int)newId;
        return _mapper.Map<Vendor>(db);
    }

    public async Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbVendor>(vendor);
        await _context.Connection.UpdateAsync(db);
        return vendor;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            "DELETE FROM Vendors WHERE Id = @Id", new { Id = id });

        return rows > 0;
    }
}
