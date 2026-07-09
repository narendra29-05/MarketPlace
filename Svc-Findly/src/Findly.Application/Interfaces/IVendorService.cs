using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface IVendorService
{
    Task<VendorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<VendorResponse>>GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<VendorResponse> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default);

    Task<VendorResponse> UpdateAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default);

    Task<VendorResponse> UpdateAddressAsync(int id, UpdateVendorAddressRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<VendorResponse> VerifyAsync(int id, string updatedBy, CancellationToken cancellationToken = default);

    Task<VendorResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default);

    
}
