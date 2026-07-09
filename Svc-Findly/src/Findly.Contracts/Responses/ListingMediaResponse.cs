using Findly.Domain.Enums;

namespace Findly.Contracts.Responses;

public class ListingMediaResponse
{
    public int Id { get; set; }
    public int ListingId { get; set; }
    public MediaType Type { get; set; }
    public string Url { get; set; } = null!;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
