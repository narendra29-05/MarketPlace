using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class VendorService : IVendorService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public VendorService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<VendorResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var vendor = await _uow.Vendors.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<IEnumerable<VendorResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var vendors = await _uow.Vendors.GetAllAsync(page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<VendorResponse>>(vendors);
    }

    public async Task<VendorResponse> CreateAsync(CreateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = Vendor.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Phone,
            request.CompanyName,
            request.CompanySize,
            request.CreatedBy,
            request.Industry,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode);

        var created = await _uow.Vendors.CreateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(created);

    }

    public async Task<VendorResponse> UpdateAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _uow.Vendors.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

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
            request.UpdatedBy);

        await _uow.Vendors.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<VendorResponse> UpdateAddressAsync(int id, UpdateVendorAddressRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _uow.Vendors.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.UpdateAddress(
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            request.UpdatedBy);

        await _uow.Vendors.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _uow.Vendors.DeleteAsync(id, cancellationToken);
    }

    public async Task<VendorResponse> VerifyAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var vendor = await _uow.Vendors.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.Verify(updatedBy);
        await _uow.Vendors.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<VendorResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default)
    {
        var vendor = await _uow.Vendors.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.Reject(reason, updatedBy);
        await _uow.Vendors.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }
}
