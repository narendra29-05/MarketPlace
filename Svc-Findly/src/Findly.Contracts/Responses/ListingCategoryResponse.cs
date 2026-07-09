namespace Findly.Contracts.Responses;

public class ListingCategoryResponse
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
}
