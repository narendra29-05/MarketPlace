using Findly.Contracts.Requests;
using Findly.Contracts.Responses;

namespace Findly.Application.Interfaces;

public interface ICategoryService
{
    Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse> ActivateAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task<CategoryResponse> DeactivateAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
