using Findly.Contracts.Category.Requests;
using Findly.Contracts.Category.Responses;

namespace Findly.Application.Categories.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<CategoryResponse>                GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryResponse>                CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse>                UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task                                  DeleteAsync(int id, CancellationToken cancellationToken = default);
}
