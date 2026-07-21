using Findly.Contracts.Common;
using Findly.Contracts.Vendor.Requests;
using Findly.Contracts.Vendor.Responses;
using Findly.Domain.Enums;

namespace Findly.Application.Vendors.Interfaces;

public interface IVendorService
{
    Task<VendorResponse>                GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<VendorResponse>                GetMyVendorAsync(CancellationToken cancellationToken = default);
    Task<PagedResponse<VendorResponse>> GetAllAsync(int page, int pageSize, VendorStatus? status = null, CancellationToken cancellationToken = default);
    Task<VendorResponse>                CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default);
    Task<VendorResponse>                UpdateAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default);
    Task<bool>                          DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<VendorResponse>                VerifyAsync(int id, CancellationToken cancellationToken = default);
    Task<VendorResponse>                RejectAsync(int id, string reason, CancellationToken cancellationToken = default);
}
