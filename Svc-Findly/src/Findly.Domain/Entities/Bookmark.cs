namespace Findly.Domain.Entities;

// A Listing saved/shortlisted by a User. One bookmark per user per listing.
public class Bookmark : Entity
{
    protected Bookmark() { }

    public int UserId { get; private set; }
    public int ListingId { get; private set; }

    public static Bookmark Create(int userId, int listingId, string createdBy)
    {
        return new Bookmark
        {
            UserId = userId,
            ListingId = listingId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
