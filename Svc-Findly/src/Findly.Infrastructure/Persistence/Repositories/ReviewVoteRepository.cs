using AutoMapper;
using Dapper;
using Dapper.Contrib.Extensions;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;
using Findly.Infrastructure.Persistence.Entities;

namespace Findly.Infrastructure.Persistence.Repositories;

internal sealed class ReviewVoteRepository : IReviewVoteRepository
{
    private readonly DatabaseContext _context;
    private readonly IMapper _mapper;

    public ReviewVoteRepository(DatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReviewVote?> GetByReviewAndUserAsync(int reviewId, int userId, CancellationToken cancellationToken)
    {
        var db = await _context.Connection.QueryFirstOrDefaultAsync<DbReviewVote>(
            new CommandDefinition("SELECT * FROM fin.ReviewVotes WHERE ReviewId = @ReviewId AND UserId = @UserId AND IsDeleted = 0",
                new { ReviewId = reviewId, UserId = userId }, _context.Transaction, cancellationToken: cancellationToken));
        return db is null ? null : _mapper.Map<ReviewVote>(db);
    }

    public async Task<IEnumerable<ReviewVote>> GetByReviewAsync(int reviewId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM fin.ReviewVotes WHERE ReviewId = @ReviewId AND IsDeleted = 0;";
        var dbs = await _context.Connection.QueryAsync<DbReviewVote>(
            new CommandDefinition(sql, new { ReviewId = reviewId }, _context.Transaction, cancellationToken: cancellationToken));
        return _mapper.Map<IEnumerable<ReviewVote>>(dbs);
    }

    public async Task<ReviewVote> CreateAsync(ReviewVote vote, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbReviewVote>(vote);
        var newId = await _context.Connection.InsertAsync(db, _context.Transaction);
        db.Id = (int)newId;
        return _mapper.Map<ReviewVote>(db);
    }

    public async Task<ReviewVote> UpdateAsync(ReviewVote vote, CancellationToken cancellationToken)
    {
        var db = _mapper.Map<DbReviewVote>(vote);
        await _context.Connection.UpdateAsync(db, _context.Transaction);
        return vote;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var rows = await _context.Connection.ExecuteAsync(
            new CommandDefinition("UPDATE fin.ReviewVotes SET IsDeleted = 1, DeletedAt = GETUTCDATE() WHERE Id = @Id AND IsDeleted = 0", new { Id = id }, _context.Transaction, cancellationToken: cancellationToken));
        return rows > 0;
    }
}
