namespace Findly.Domain.Entities;

// Join entity: a Listing can belong to many Categories, and a Category can
// contain many Listings.
public class ListingCategory : Entity
{
    protected ListingCategory() { }

    public int ListingId { get; private set; }
    public int CategoryId { get; private set; }

    public static ListingCategory Create(int listingId, int categoryId, string createdBy)
    {
        return new ListingCategory
        {
            ListingId = listingId,
            CategoryId = categoryId,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
