namespace Findly.Contracts.Responses;

public class ListingFeatureResponse
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsHighlighted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
