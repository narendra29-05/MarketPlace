using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public CategoryRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbCategory>(
            "SELECT * FROM [fin].[Categories] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Category>(db);
    }

    public async Task<Category?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbCategory>(
            "SELECT * FROM [fin].[Categories] WHERE Slug = @Slug",
            new { Slug = slug },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<Category>(db);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var dbs = await _context.Connection.QueryAsync<DbCategory>(
            "SELECT * FROM [fin].[Categories] WHERE (@IncludeInactive = 1 OR IsActive = 1) ORDER BY Name",
            new { IncludeInactive = includeInactive },
            _context.CurrentTransaction);

        return _mapper.Map<List<Category>>(dbs);
    }

    public async Task<IReadOnlyList<Category>> GetByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
            return [];

        var dbs = await _context.Connection.QueryAsync<DbCategory>(
            "SELECT * FROM [fin].[Categories] WHERE Id IN @Ids",
            new { Ids = ids },
            _context.CurrentTransaction);

        return _mapper.Map<List<Category>>(dbs);
    }

    public async Task<IReadOnlyList<Category>> GetByListingIdAsync(int listingId, CancellationToken cancellationToken)
    {
        var dbs = await _context.Connection.QueryAsync<DbCategory>(
            """
            SELECT c.* FROM [fin].[Categories] c
            JOIN [fin].[ListingCategories] lc ON lc.CategoryId = c.Id
            WHERE lc.ListingId = @ListingId
            ORDER BY c.Name
            """,
            new { ListingId = listingId },
            _context.CurrentTransaction);

        return _mapper.Map<List<Category>>(dbs);
    }

    public async Task<IReadOnlyList<ListingCategoryLink>> GetByListingIdsAsync(IReadOnlyCollection<int> listingIds, CancellationToken cancellationToken)
    {
        if (listingIds.Count == 0)
            return [];

        var links = (await _context.Connection.QueryAsync<LinkRow>(
            "SELECT ListingId, CategoryId FROM [fin].[ListingCategories] WHERE ListingId IN @ListingIds",
            new { ListingIds = listingIds },
            _context.CurrentTransaction)).ToList();

        if (links.Count == 0)
            return [];

        var categories = await GetByIdsAsync(links.Select(l => l.CategoryId).Distinct().ToList(), cancellationToken);
        var byId = categories.ToDictionary(c => c.Id);

        return links
            .Where(l => byId.ContainsKey(l.CategoryId))
            .Select(l => new ListingCategoryLink(l.ListingId, byId[l.CategoryId]))
            .ToList();
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbCategory>(category);
        var newId = await _context.Connection.InsertAsync(db, _context.CurrentTransaction);
        db.Id = (int)newId;
        return _mapper.Map<Category>(db);
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbCategory>(category);
        await _context.Connection.UpdateAsync(db, _context.CurrentTransaction);
        return category;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            "DELETE FROM [fin].[Categories] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return rows > 0;
    }

    private sealed class LinkRow
    {
        public int ListingId { get; set; }
        public int CategoryId { get; set; }
    }
}
