using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface ILeadService
{
    Task<LeadResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeadResponse>> GetByVendorAsync(int vendorId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<LeadResponse> CreateAsync(CreateLeadRequest request, CancellationToken cancellationToken = default);
    Task<LeadResponse> UpdateStatusAsync(int id, UpdateLeadStatusRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
