namespace Findly.Domain.Entities;

// A single feature/capability advertised by a Listing
// (e.g. "Single Sign-On", "REST API", "24/7 Support").
public class ListingFeature : Entity
{
    protected ListingFeature() { }

    public int ListingId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsHighlighted { get; private set; }

    public static ListingFeature Create(
        int listingId,
        string name,
        string createdBy,
        string? description = null,
        bool isHighlighted = false)
    {
        return new ListingFeature
        {
            ListingId = listingId,
            Name = name,
            Description = description,
            IsHighlighted = isHighlighted,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string updatedBy, string? description, bool isHighlighted)
    {
        Name = name;
        Description = description;
        IsHighlighted = isHighlighted;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
