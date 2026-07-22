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

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbUser>(
            "SELECT * FROM [fin].[Users] WHERE Id = @Id",
            new { Id = id },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<User>(db);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbUser>(
            "SELECT * FROM [fin].[Users] WHERE Email = @Email",
            new { Email = email.Trim().ToLowerInvariant() },
            _context.CurrentTransaction);

        return db is null ? null : _mapper.Map<User>(db);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbUser>(user);
        var newId = await _context.Connection.InsertAsync(db, _context.CurrentTransaction);
        db.Id = (int)newId;
        return _mapper.Map<User>(db);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbUser>(user);
        await _context.Connection.UpdateAsync(db, _context.CurrentTransaction);
        return user;
    }
}
