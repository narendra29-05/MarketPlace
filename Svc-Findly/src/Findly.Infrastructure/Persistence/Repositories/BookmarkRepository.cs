using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class BookmarkRepository : IBookmarkRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public BookmarkRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Bookmark> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbBookmark>(
            new CommandDefinition("SELECT * FROM fin.Bookmarks WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<Bookmark>(db);
    }

    public async Task<IEnumerable<Bookmark>> GetByUserAsync(int userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        var offset = (page - 1) * pageSize;

        const string sql = "SELECT * FROM fin.Bookmarks WHERE UserId = @UserId AND IsDeleted = 0 ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        var dbs = await _context.Connection.QueryAsync<DbBookmark>(
            new CommandDefinition(sql, new { UserId = userId, Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<Bookmark>>(dbs);
    }

    public async Task<Bookmark> CreateAsync(Bookmark bookmark, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbBookmark>(bookmark);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<Bookmark>(db);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.Bookmarks SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
