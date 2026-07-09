using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbUser>(
            new CommandDefinition("SELECT * FROM fin.Users WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<User>(db);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbUser>(
            new CommandDefinition("SELECT * FROM fin.Users WHERE Email = @Email AND IsDeleted = 0", new { Email = email }, _context.Transaction, cancellationToken: cancellationToken));
        return db is null ? null : _mapper.Map<User>(db);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        var offset = (page - 1) * pageSize;

        const string sql = "SELECT * FROM fin.Users WHERE IsDeleted = 0 ORDER BY Id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
        var dbs = await _context.Connection.QueryAsync<DbUser>(
            new CommandDefinition(sql, new { Offset = offset, PageSize = pageSize }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<User>>(dbs);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbUser>(user);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<User>(db);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbUser>(user);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return user;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.Users SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
