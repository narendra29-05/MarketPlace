using AutoMapper;
using Findly.Application.Auth.Interfaces;
using Findly.Application.Common.Exceptions;
using Findly.Application.Common.Interfaces;
using Findly.Contracts.Auth.Requests;
using Findly.Contracts.Auth.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository    _userRepository;
    private readonly IPasswordHasher    _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ICurrentUser       _currentUser;
    private readonly IMapper            _mapper;

    public AuthService(
        IUserRepository    userRepository,
        IPasswordHasher    passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        ICurrentUser       currentUser,
        IMapper            mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _currentUser    = currentUser;
        _mapper         = mapper;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("A user with this email is already registered.");

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            _passwordHasher.Hash(request.Password),
            request.Role,
            createdBy: request.Email.Trim().ToLowerInvariant());

        var created = await _userRepository.CreateAsync(user, cancellationToken);
        return BuildAuthResponse(created);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new AuthenticationFailedException("Invalid email or password.");

        return BuildAuthResponse(user);
    }

    public async Task<UserResponse> GetMeAsync(CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId
            ?? throw new AuthenticationFailedException("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        return _mapper.Map<UserResponse>(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAtUtc) = _tokenGenerator.Generate(user);

        return new AuthResponse
        {
            Token        = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId       = user.Id,
            Email        = user.EmailAddress.Value,
            FirstName    = user.FirstName,
            LastName     = user.LastName,
            Role         = user.Role,
            VendorId     = user.VendorId
        };
    }
}
