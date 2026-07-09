namespace Findly.Contracts.Responses;

public class ListingTagResponse
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public int TagId { get; set; }
    public DateTime CreatedAt { get; set; }
}
