using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
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

    public async Task<Vendor?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbVendor>(
            "SELECT * FROM [fin].[Vendors] WHERE Id = @Id", new { Id = id });

        return db is null ? null : _mapper.Map<Vendor>(db);
    }

    public async Task<PagedResult<Vendor>> GetAllAsync(int page, int pageSize, VendorStatus? status, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Vendors] WHERE (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Vendors]
            WHERE (@Status IS NULL OR [Status] = @Status)
            ORDER BY [Id]
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        using var multi = await _context.Connection.QueryMultipleAsync(
            sql, new { Status = (int?)status, Page = page, PageSize = pageSize });

        var totalCount = await multi.ReadSingleAsync<int>();
        var dbs        = await multi.ReadAsync<DbVendor>();

        return new PagedResult<Vendor>(_mapper.Map<List<Vendor>>(dbs), totalCount, page, pageSize);
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
            "DELETE FROM [fin].[Vendors] WHERE Id = @Id", new { Id = id });

        return rows > 0;
    }
}
