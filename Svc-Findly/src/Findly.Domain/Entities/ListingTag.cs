namespace Findly.Domain.Entities;

// Join entity: a Listing can carry many Tags, and a Tag can apply to many Listings.
public class ListingTag : Entity
{
    protected ListingTag() { }

    public int ListingId { get; private set; }
    public int TagId { get; private set; }

    public static ListingTag Create(int listingId, int tagId, string createdBy)
    {
        return new ListingTag
        {
            ListingId = listingId,
            TagId = tagId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
