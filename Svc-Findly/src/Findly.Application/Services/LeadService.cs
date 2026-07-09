using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class LeadService : ILeadService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LeadService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<LeadResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var lead = await _uow.Leads.GetByIdAsync(id, cancellationToken);
        if (lead is null)
            throw new KeyNotFoundException($"Lead with id {id} not found.");
        return _mapper.Map<LeadResponse>(lead);
    }

    public async Task<IEnumerable<LeadResponse>> GetByVendorAsync(int vendorId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var leads = await _uow.Leads.GetByVendorAsync(vendorId, page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<LeadResponse>>(leads);
    }

    public async Task<LeadResponse> CreateAsync(CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        var lead = Lead.Create(
            request.ListingId,
            request.VendorId,
            request.Type,
            request.Name,
            request.Email,
            request.CreatedBy,
            request.UserId,
            request.Phone,
            request.CompanyName,
            request.Message);

        var created = await _uow.Leads.CreateAsync(lead, cancellationToken);
        return _mapper.Map<LeadResponse>(created);
    }

    public async Task<LeadResponse> UpdateStatusAsync(int id, UpdateLeadStatusRequest request, CancellationToken cancellationToken = default)
    {
        var lead = await _uow.Leads.GetByIdAsync(id, cancellationToken);
        if (lead is null)
            throw new KeyNotFoundException($"Lead with id {id} not found.");

        lead.UpdateStatus(request.Status, request.UpdatedBy);
        await _uow.Leads.UpdateAsync(lead, cancellationToken);
        return _mapper.Map<LeadResponse>(lead);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Leads.DeleteAsync(id, cancellationToken);
    }
}
