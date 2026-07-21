using System.Data;

namespace Findly.Infrastructure.Persistence;

internal sealed class DatabaseContext : IDisposable
{
    private readonly IDbConnection _connection;
    private bool                   _disposed;

    public DatabaseContext(IDbConnection connection)
    {
        _connection = connection;
    }

    public IDbConnection Connection => _connection;

    public IDbTransaction? CurrentTransaction { get; private set; }

    public void BeginTransaction()
    {
        if (CurrentTransaction is not null)
            throw new InvalidOperationException("A transaction is already in progress.");

        CurrentTransaction = _connection.BeginTransaction();
    }

    public void Commit()
    {
        if (CurrentTransaction is null)
            throw new InvalidOperationException("No transaction in progress.");

        CurrentTransaction.Commit();
        CurrentTransaction.Dispose();
        CurrentTransaction = null;
    }

    public void Rollback()
    {
        if (CurrentTransaction is null)
            return;

        CurrentTransaction.Rollback();
        CurrentTransaction.Dispose();
        CurrentTransaction = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        CurrentTransaction?.Dispose();
        _disposed = true;
    }
}
