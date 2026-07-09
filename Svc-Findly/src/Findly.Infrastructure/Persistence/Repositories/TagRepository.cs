using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class TagRepository : ITagRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public TagRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Tag> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbTag>(
            new CommandDefinition("SELECT * FROM fin.Tags WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<Tag>(db);
    }

    public async Task<IEnumerable<Tag>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        var offset = (page - 1) * pageSize;

        const string sql = "SELECT * FROM fin.Tags WHERE IsDeleted = 0 ORDER BY Name OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        var dbs = await _context.Connection.QueryAsync<DbTag>(
            new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<Tag>>(dbs);
    }

    public async Task<Tag> CreateAsync(Tag tag, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbTag>(tag);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<Tag>(db);
    }

    public async Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbTag>(tag);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return tag;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.Tags SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
