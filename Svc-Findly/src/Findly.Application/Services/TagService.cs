using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TagService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<TagResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tag = await _uow.Tags.GetByIdAsync(id, cancellationToken);
        if (tag is null)
            throw new KeyNotFoundException($"Tag with id {id} not found.");
        return _mapper.Map<TagResponse>(tag);
    }

    public async Task<IEnumerable<TagResponse>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var tags = await _uow.Tags.GetAllAsync(page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<TagResponse>>(tags);
    }

    public async Task<TagResponse> CreateAsync(CreateTagRequest request, CancellationToken cancellationToken = default)
    {
        var tag = Tag.Create(request.Name, request.Slug, request.Type, request.CreatedBy);
        var created = await _uow.Tags.CreateAsync(tag, cancellationToken);
        return _mapper.Map<TagResponse>(created);
    }

    public async Task<TagResponse> UpdateAsync(int id, UpdateTagRequest request, CancellationToken cancellationToken = default)
    {
        var tag = await _uow.Tags.GetByIdAsync(id, cancellationToken);
        if (tag is null)
            throw new KeyNotFoundException($"Tag with id {id} not found.");

        tag.Update(request.Name, request.Slug, request.Type, request.UpdatedBy);
        await _uow.Tags.UpdateAsync(tag, cancellationToken);
        return _mapper.Map<TagResponse>(tag);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Tags.DeleteAsync(id, cancellationToken);
    }
}
