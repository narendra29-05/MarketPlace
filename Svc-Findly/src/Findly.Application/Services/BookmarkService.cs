using AutoMapper;
using Findly.Application.Interfaces;
using Findly.Contracts.Requests;
using Findly.Contracts.Responses;
using Findly.Domain.Entities;
using Findly.Domain.Repositories;

namespace Findly.Application.Services;

public class BookmarkService : IBookmarkService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public BookmarkService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookmarkResponse>> GetByUserAsync(int userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var bookmarks = await _uow.Bookmarks.GetByUserAsync(userId, page, pageSize, cancellationToken);
        return _mapper.Map<IEnumerable<BookmarkResponse>>(bookmarks);
    }

    public async Task<BookmarkResponse> CreateAsync(CreateBookmarkRequest request, CancellationToken cancellationToken = default)
    {
        var bookmark = Bookmark.Create(request.UserId, request.ListingId, request.CreatedBy);
        var created = await _uow.Bookmarks.CreateAsync(bookmark, cancellationToken);
        return _mapper.Map<BookmarkResponse>(created);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _uow.Bookmarks.DeleteAsync(id, cancellationToken);
    }
}
