using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

// A screenshot or video in a Listing's gallery.
public class ListingMedia : Entity
{
    protected ListingMedia() { }

    public int ListingId { get; private set; }
    public MediaType Type { get; private set; }
    public string Url { get; private set; } = null!;
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }

    public static ListingMedia Create(
        int listingId,
        MediaType type,
        string url,
        string createdBy,
        string? caption = null,
        int displayOrder = 0)
    {
        return new ListingMedia
        {
            ListingId = listingId,
            Type = type,
            Url = url,
            Caption = caption,
            DisplayOrder = displayOrder,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string url, MediaType type, string updatedBy, string? caption, int displayOrder)
    {
        Url = url;
        Type = type;
        Caption = caption;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
