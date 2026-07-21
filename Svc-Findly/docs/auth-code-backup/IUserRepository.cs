using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User> CreateAsync(User user, CancellationToken cancellationToken);

    Task<User> UpdateAsync(User user, CancellationToken cancellationToken);
}
