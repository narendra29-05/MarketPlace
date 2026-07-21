using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.Domain.Repositories;

public sealed record LeadWithListing(Lead Lead, string ListingName);

public interface ILeadRepository
{
    Task<Lead?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<PagedResult<LeadWithListing>> GetByVendorIdAsync(int vendorId, LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<PagedResult<LeadWithListing>> GetAllAsync(LeadStatus? status, int page, int pageSize, CancellationToken cancellationToken);

    Task<Lead> CreateAsync(Lead lead, CancellationToken cancellationToken);

    Task<Lead> UpdateAsync(Lead lead, CancellationToken cancellationToken);
}
