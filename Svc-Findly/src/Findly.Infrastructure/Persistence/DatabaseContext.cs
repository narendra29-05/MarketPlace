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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}
