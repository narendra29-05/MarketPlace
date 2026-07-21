using Findly.Domain.Entities;
using Findly.Domain.Enums;

namespace Findly.Domain.Repositories;

public interface IVendorRepository
{
    Task<Vendor?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<PagedResult<Vendor>> GetAllAsync(int page, int pageSize, VendorStatus? status, CancellationToken cancellationToken);

    Task<Vendor> CreateAsync(Vendor vendor, CancellationToken cancellationToken);

    Task<Vendor> UpdateAsync(Vendor vendor, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
