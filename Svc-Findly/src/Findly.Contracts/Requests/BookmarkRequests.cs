namespace Findly.Contracts.Requests;

public class CreateBookmarkRequest
{
    public int UserId { get; set; }
    public int ListingId { get; set; }
    public string CreatedBy { get; set; } = null!;
}
