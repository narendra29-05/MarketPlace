namespace Findly.Contracts.Common;

public class PagedResponse<T>
{
    public IReadOnlyList<T> Items      { get; set; } = [];
    public int              TotalCount { get; set; }
    public int              Page       { get; set; }
    public int              PageSize   { get; set; }
    public int              TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    public bool             HasNext    => Page < TotalPages;
    public bool             HasPrev    => Page > 1;
}
