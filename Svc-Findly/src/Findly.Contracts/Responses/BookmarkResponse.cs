namespace Findly.Contracts.Responses;

public class BookmarkResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ListingId { get; set; }
    public DateTime CreatedAt { get; set; }
}
