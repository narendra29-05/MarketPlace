using Findly.Domain.Entities;

namespace Findly.Domain.Repositories;

public interface ILeadRepository
{
    Task<Lead> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Lead>> GetByVendorAsync(int vendorId, int page, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<Lead>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Lead> CreateAsync(Lead lead, CancellationToken cancellationToken);
    Task<Lead> UpdateAsync(Lead lead, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
