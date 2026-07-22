using AutoMapper;
using Findly.Application.Common.Exceptions;
using Findly.Application.Common.Interfaces;
using Findly.Application.Common.Mappings;
using Findly.Application.Vendors.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Vendor.Requests;
using Findly.Contracts.Vendor.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;

namespace Findly.Application.Vendors.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IMapper _mapper;

    public VendorService(
        IVendorRepository repository,
        IUserRepository userRepository,
        ICurrentUser currentUser,
        IMapper mapper)
    {
        _repository = repository;
        _userRepository = userRepository;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<VendorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<VendorResponse> GetMyVendorAsync(CancellationToken cancellationToken = default)
    {
        var vendorId = await ResolveVendorIdAsync(cancellationToken);
        if (vendorId is null)
            throw new KeyNotFoundException("No vendor profile found for the current user.");

        return await GetByIdAsync(vendorId.Value, cancellationToken);
    }

    public async Task<PagedResponse<VendorResponse>> GetAllAsync(int page, int pageSize, VendorStatus? status = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _repository.GetAllAsync(page, pageSize, status, cancellationToken);
        return result.ToPagedResponse<Vendor, VendorResponse>(_mapper);
    }

    public async Task<VendorResponse> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var userId = _currentUser.UserId
            ?? throw new AuthenticationFailedException("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            throw new KeyNotFoundException($"User with id {userId} not found.");

        if (user.VendorId.HasValue)
            throw new InvalidOperationException("Current user already has a vendor profile.");

        var vendor = Vendor.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.CompanyName,
            request.CompanySize,
            _currentUser.AuditName,
            request.Industry,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode);

        var createdVendor = await _repository.CreateAsync(vendor, cancellationToken);

        user.LinkVendor(createdVendor.Id, _currentUser.AuditName);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return _mapper.Map<VendorResponse>(createdVendor);
    }

    public async Task<VendorResponse> UpdateAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        await EnsureCanManageAsync(id, cancellationToken);

        vendor.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.CompanyName,
            request.CompanySize,
            request.Industry,
            request.WebsiteUrl,
            request.LogoUrl,
            _currentUser.AuditName);

        vendor.UpdateAddress(
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            _currentUser.AuditName);

        await _repository.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<VendorResponse> VerifyAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.Verify(_currentUser.AuditName);
        await _repository.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<VendorResponse> RejectAsync(int id, string reason, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.Reject(reason, _currentUser.AuditName);
        await _repository.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    private async Task<int?> ResolveVendorIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUser.VendorId.HasValue)
            return _currentUser.VendorId;

        if (_currentUser.UserId is not int userId)
            return null;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.VendorId;
    }

    private async Task EnsureCanManageAsync(int vendorId, CancellationToken cancellationToken)
    {
        if (_currentUser.IsAdmin)
            return;

        var ownVendorId = await ResolveVendorIdAsync(cancellationToken);
        if (ownVendorId != vendorId)
            throw new UnauthorizedAccessException("You can only manage your own vendor profile.");
    }
}
