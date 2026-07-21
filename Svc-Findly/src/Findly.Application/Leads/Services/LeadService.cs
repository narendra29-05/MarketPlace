using AutoMapper;
using Findly.Application.Common.Exceptions;
using Findly.Application.Common.Interfaces;
using Findly.Application.Leads.Interfaces;
using Findly.Contracts.Common;
using Findly.Contracts.Lead.Requests;
using Findly.Contracts.Lead.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Enums;
using Findly.Domain.Repositories;

namespace Findly.Application.Leads.Services;

public class LeadService : ILeadService
{
    private readonly ILeadRepository    _repository;
    private readonly IListingRepository _listingRepository;
    private readonly IUserRepository    _userRepository;
    private readonly ICurrentUser       _currentUser;
    private readonly IMapper            _mapper;

    public LeadService(
        ILeadRepository    repository,
        IListingRepository listingRepository,
        IUserRepository    userRepository,
        ICurrentUser       currentUser,
        IMapper            mapper)
    {
        _repository        = repository;
        _listingRepository = listingRepository;
        _userRepository    = userRepository;
        _currentUser       = currentUser;
        _mapper            = mapper;
    }

    public async Task<LeadResponse> SubmitAsync(int listingId, CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        var listing = await _listingRepository.GetByIdAsync(listingId, cancellationToken);
        if (listing is null)
            throw new KeyNotFoundException($"Listing with id {listingId} not found.");

        if (listing.Status != ListingStatus.Published)
            throw new InvalidOperationException("Leads can only be submitted for published listings.");

        var lead = Lead.Create(
            listingId,
            listing.VendorId,
            request.LeadType,
            request.FullName,
            request.BusinessEmail,
            request.Phone,
            request.Company,
            request.CompanySize,
            request.Message,
            _currentUser.AuditName);

        var created = await _repository.CreateAsync(lead, cancellationToken);

        var response = _mapper.Map<LeadResponse>(created);
        response.ListingName = listing.Name;
        return response;
    }

    public async Task<LeadResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var lead = await GetLeadOrThrowAsync(id, cancellationToken);
        await EnsureVendorOwnershipAsync(lead, cancellationToken);

        var listing  = await _listingRepository.GetByIdAsync(lead.ListingId, cancellationToken);
        var response = _mapper.Map<LeadResponse>(lead);
        response.ListingName = listing?.Name;
        return response;
    }

    public async Task<PagedResponse<LeadResponse>> GetMyLeadsAsync(LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var vendorId = await RequireVendorIdAsync(cancellationToken);

        var result = await _repository.GetByVendorIdAsync(vendorId, status, page, pageSize, cancellationToken);
        return ToPagedResponse(result);
    }

    public async Task<PagedResponse<LeadResponse>> GetAllAsync(LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page     = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var result = await _repository.GetAllAsync(status, page, pageSize, cancellationToken);
        return ToPagedResponse(result);
    }

    public async Task<LeadResponse> UpdateStatusAsync(int id, LeadStatus status, CancellationToken cancellationToken = default)
    {
        var lead = await GetLeadOrThrowAsync(id, cancellationToken);
        await EnsureVendorOwnershipAsync(lead, cancellationToken);

        lead.TransitionTo(status, _currentUser.AuditName);
        await _repository.UpdateAsync(lead, cancellationToken);

        var listing  = await _listingRepository.GetByIdAsync(lead.ListingId, cancellationToken);
        var response = _mapper.Map<LeadResponse>(lead);
        response.ListingName = listing?.Name;
        return response;
    }

    // =========================================================================
    // Helpers
    // =========================================================================

    private async Task<Lead> GetLeadOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var lead = await _repository.GetByIdAsync(id, cancellationToken);
        if (lead is null)
            throw new KeyNotFoundException($"Lead with id {id} not found.");

        return lead;
    }

    private async Task<int> RequireVendorIdAsync(CancellationToken cancellationToken)
    {
        if (_currentUser.VendorId.HasValue)
            return _currentUser.VendorId.Value;

        var userId = _currentUser.UserId
            ?? throw new AuthenticationFailedException("Not authenticated.");

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user?.VendorId
            ?? throw new InvalidOperationException("Current user has no vendor profile.");
    }

    private async Task EnsureVendorOwnershipAsync(Lead lead, CancellationToken cancellationToken)
    {
        if (_currentUser.IsAdmin)
            return;

        var vendorId = await RequireVendorIdAsync(cancellationToken);
        if (lead.VendorId != vendorId)
            throw new UnauthorizedAccessException("You can only manage leads for your own listings.");
    }

    private PagedResponse<LeadResponse> ToPagedResponse(PagedResult<LeadWithListing> result)
    {
        var items = result.Items
            .Select(x =>
            {
                var response = _mapper.Map<LeadResponse>(x.Lead);
                response.ListingName = x.ListingName;
                return response;
            })
            .ToList();

        return new PagedResponse<LeadResponse>
        {
            Items      = items,
            TotalCount = result.TotalCount,
            Page       = result.Page,
            PageSize   = result.PageSize
        };
    }
}
