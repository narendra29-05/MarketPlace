using AutoMapper;
using Findly.Application.Categories.Interfaces;
using Findly.Application.Common.Interfaces;
using Findly.Contracts.Category.Requests;
using Findly.Contracts.Category.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ICurrentUser        _currentUser;
    private readonly IMapper             _mapper;

    public CategoryService(ICategoryRepository repository, ICurrentUser currentUser, IMapper mapper)
    {
        _repository  = repository;
        _currentUser = currentUser;
        _mapper      = mapper;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(includeInactive, cancellationToken);
        return _mapper.Map<List<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetBySlugAsync(request.Slug, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException("A category with this slug already exists.");

        var category = Category.Create(
            request.Name,
            request.Slug,
            request.Description,
            request.IconUrl,
            _currentUser.AuditName);

        var created = await _repository.CreateAsync(category, cancellationToken);
        return _mapper.Map<CategoryResponse>(created);
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new KeyNotFoundException($"Category with id {id} not found.");

        var existing = await _repository.GetBySlugAsync(request.Slug, cancellationToken);
        if (existing is not null && existing.Id != id)
            throw new InvalidOperationException("A category with this slug already exists.");

        category.UpdateDetails(
            request.Name,
            request.Slug,
            request.Description,
            request.IconUrl,
            request.IsActive,
            _currentUser.AuditName);

        await _repository.UpdateAsync(category, cancellationToken);
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        if (!deleted)
            throw new KeyNotFoundException($"Category with id {id} not found.");
    }
}
