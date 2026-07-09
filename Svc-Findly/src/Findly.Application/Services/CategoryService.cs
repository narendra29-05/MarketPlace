using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found.");
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var categories = await _uow.Categories.GetAllAsync(page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = Category.Create(
            request.Name,
            request.Slug,
            request.CreatedBy,
            request.Description,
            request.IconUrl,
            request.ParentCategoryId,
            request.DisplayOrder);

        var created = await _uow.Categories.CreateAsync(category, cancellationToken);
        return _mapper.Map<CategoryResponse>(created);
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.UpdateDetails(
            request.Name,
            request.Slug,
            request.UpdatedBy,
            request.Description,
            request.IconUrl,
            request.ParentCategoryId,
            request.DisplayOrder);

        await _uow.Categories.UpdateAsync(category, cancellationToken);
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> ActivateAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Activate(updatedBy);
        await _uow.Categories.UpdateAsync(category, cancellationToken);
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> DeactivateAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var category = await _uow.Categories.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        category.Deactivate(updatedBy);
        await _uow.Categories.UpdateAsync(category, cancellationToken);
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Categories.DeleteAsync(id, cancellationToken);
    }
}
