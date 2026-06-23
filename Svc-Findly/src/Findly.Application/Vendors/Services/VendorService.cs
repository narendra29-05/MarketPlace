using AutoMapper;
using Findly.Application.Vendors.Interfaces;
using Findly.Contracts.Vendor.Requests;
using Findly.Contracts.Vendor.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Vendors.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _repository;
    private readonly IMapper           _mapper;

    public VendorService(IVendorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper     = mapper;
    }

    public async Task<VendorResponse> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<IEnumerable<VendorResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var vendors = await _repository.GetAllAsync(cancellationToken);
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
            "system",
            request.Industry,
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode);
    
        var createdVendor = await _repository.CreateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(createdVendor);
    }

    public async Task<VendorResponse> UpdateAsync(int id, UpdateVendorRequest request, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
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

        vendor.UpdateAddress(
            request.AddressLine1,
            request.AddressLine2,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            request.UpdatedBy);

        await _repository.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<VendorResponse> VerifyAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.Verify(updatedBy);
        await _repository.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }

    public async Task<VendorResponse> RejectAsync(int id, string reason, string updatedBy, CancellationToken cancellationToken = default)
    {
        var vendor = await _repository.GetByIdAsync(id, cancellationToken);
        if (vendor is null)
            throw new KeyNotFoundException($"Vendor with id {id} not found.");

        vendor.Reject(reason, updatedBy);
        await _repository.UpdateAsync(vendor, cancellationToken);
        return _mapper.Map<VendorResponse>(vendor);
    }
}
