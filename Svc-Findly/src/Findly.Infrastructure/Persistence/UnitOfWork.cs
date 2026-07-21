using Findly.Domain.Repositories;

namespace Findly.Infrastructure.Persistence;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;

    public UnitOfWork(DatabaseContext context)
    {
        _context = context;
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        _context.BeginTransaction();

        try
        {
            await action();
            _context.Commit();
        }
        catch
        {
            _context.Rollback();
            throw;
        }
    }
}
