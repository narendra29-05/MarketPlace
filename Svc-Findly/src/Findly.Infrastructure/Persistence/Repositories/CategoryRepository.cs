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

    public async Task<Category> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbCategory>(
            new CommandDefinition("SELECT * FROM fin.Categories WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<Category>(db);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        var offset = (page - 1) * pageSize;

        const string sql = "SELECT * FROM fin.Categories WHERE IsDeleted = 0 ORDER BY DisplayOrder, Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        var dbs = await _context.Connection.QueryAsync<DbCategory>(
            new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<Category>>(dbs);
    }

    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbCategory>(category);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<Category>(db);
    }

    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbCategory>(category);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return category;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.Categories SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
