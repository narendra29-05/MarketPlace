using Findly.Contracts.Common;
using Findly.Contracts.Lead.Requests;
using Findly.Contracts.Lead.Responses;
using Findly.Domain.Enums;

namespace Findly.Application.Leads.Interfaces;

public interface ILeadService
{
    Task<LeadResponse>                SubmitAsync(int listingId, CreateLeadRequest request, CancellationToken cancellationToken = default);
    Task<LeadResponse>                GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResponse<LeadResponse>> GetMyLeadsAsync(LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResponse<LeadResponse>> GetAllAsync(LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<LeadResponse>                UpdateStatusAsync(int id, LeadStatus status, CancellationToken cancellationToken = default);
}
