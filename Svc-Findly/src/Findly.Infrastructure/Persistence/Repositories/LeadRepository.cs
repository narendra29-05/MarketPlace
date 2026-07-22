using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class LeadRepository : ILeadRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public LeadRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Lead?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbLead>(
            "SELECT * FROM [fin].[Leads] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Lead>(db);
    }

    public async Task<PagedResult<LeadWithListing>> GetByVendorIdAsync(int vendorId, LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Leads]
            WHERE VendorId = @VendorId AND (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Leads]
            WHERE VendorId = @VendorId AND (@Status IS NULL OR [Status] = @Status)
            ORDER BY [CreatedAt] DESC
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        return await QueryPagedAsync(sql, new
        {
            VendorId = vendorId,
            Status = (int?)status,
            Page = page,
            PageSize = pageSize
        }, page, pageSize);
    }

    public async Task<PagedResult<LeadWithListing>> GetAllAsync(LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT COUNT(*) FROM [fin].[Leads]
            WHERE (@Status IS NULL OR [Status] = @Status);

            SELECT * FROM [fin].[Leads]
            WHERE (@Status IS NULL OR [Status] = @Status)
            ORDER BY [CreatedAt] DESC
            OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        return await QueryPagedAsync(sql, new
        {
            Status = (int?)status,
            Page = page,
            PageSize = pageSize
        }, page, pageSize);
    }

    public async Task<Lead> CreateAsync(Lead lead, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbLead>(lead);
        var newId = await _context.Connection.InsertAsync(db, _context.CurrentTransaction);
        db.Id = (int)newId;
        return _mapper.Map<Lead>(db);
    }

    public async Task<Lead> UpdateAsync(Lead lead, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbLead>(lead);
        await _context.Connection.UpdateAsync(db, _context.CurrentTransaction);
        return lead;
    }

    private async Task<PagedResult<LeadWithListing>> QueryPagedAsync(string sql, object parameters, int page, int pageSize)
    {
        using var multi = await _context.Connection.QueryMultipleAsync(sql, parameters, _context.CurrentTransaction);

        var totalCount = await multi.ReadSingleAsync<int>();
        var dbs = (await multi.ReadAsync<DbLead>()).ToList();

        var listingNames = await GetListingNamesAsync(dbs.Select(d => d.ListingId).Distinct().ToList());

        var items = dbs
            .Select(db => new LeadWithListing(
                _mapper.Map<Lead>(db),
                listingNames.GetValueOrDefault(db.ListingId, string.Empty)))
            .ToList();

        return new PagedResult<LeadWithListing>(items, totalCount, page, pageSize);
    }

    private async Task<Dictionary<int, string>> GetListingNamesAsync(IReadOnlyCollection<int> listingIds)
    {
        if (listingIds.Count == 0)
            return [];

        var rows = await _context.Connection.QueryAsync<NameRow>(
            "SELECT Id, Name FROM [fin].[Listings] WHERE Id IN @ListingIds",
            new { ListingIds = listingIds },
            _context.CurrentTransaction);

        return rows.ToDictionary(r => r.Id, r => r.Name);
    }

    private sealed class NameRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
