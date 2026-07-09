using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found.");
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var users = await _uow.Users.GetAllAsync(page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // NOTE: password should be hashed by a dedicated auth flow before reaching here.
        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Role,
            request.CreatedBy,
            request.JobTitle,
            request.CompanyName,
            request.IndustryType);

        var created = await _uow.Users.CreateAsync(user, cancellationToken);
        return _mapper.Map<UserResponse>(created);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.UpdateProfile(
            request.FirstName,
            request.LastName,
            request.UpdatedBy,
            request.AvatarUrl,
            request.JobTitle,
            request.CompanyName,
            request.IndustryType);

        await _uow.Users.UpdateAsync(user, cancellationToken);
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> ChangePasswordAsync(int id, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.ChangePassword(request.Password, request.UpdatedBy);
        await _uow.Users.UpdateAsync(user, cancellationToken);
        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> VerifyEmailAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Users.GetByIdAsync(id, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {id} not found.");

        user.VerifyEmail(updatedBy);
        await _uow.Users.UpdateAsync(user, cancellationToken);
        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Users.DeleteAsync(id, cancellationToken);
    }
}
