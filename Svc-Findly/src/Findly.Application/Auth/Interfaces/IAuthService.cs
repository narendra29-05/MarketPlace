using Findly.Contracts.Auth.Requests;
using Findly.Contracts.Auth.Responses;

namespace Findly.Application.Auth.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse> GetMeAsync(CancellationToken cancellationToken = default);
}
