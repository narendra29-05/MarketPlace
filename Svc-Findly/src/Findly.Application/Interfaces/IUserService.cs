using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> ChangePasswordAsync(int id, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<UserResponse> VerifyEmailAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
