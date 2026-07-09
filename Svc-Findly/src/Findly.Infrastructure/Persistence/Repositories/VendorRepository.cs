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
    private readonly IMapper _mapper;

    public VendorRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Vendor> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbVendor>(
            new CommandDefinition("SELECT * FROM fin.Vendors WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));

        return _mapper.Map<Vendor>(db);
    }

    public async Task<IEnumerable<Vendor>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        var offset = (page - 1) * pageSize;

        const string sql = """
            SELECT *
            FROM fin.Vendors
            WHERE IsDeleted = 0
            ORDER BY Id
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
            """;

        var dbs = await _context.Connection.QueryAsync<DbVendor>(
            new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));

        return _mapper.Map<IEnumerable<Vendor>>(dbs);
    }

    public async Task<Vendor> CreateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbVendor>(vendor);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<Vendor>(db);

    }

    public async Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbVendor>(vendor);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return vendor;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition(
                "UPDATE fin.Vendors SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0",
                new { Id = id },
                _context.Transaction,
                cancellationToken: cancellationToken));

        return rows > 0;
    }
}
