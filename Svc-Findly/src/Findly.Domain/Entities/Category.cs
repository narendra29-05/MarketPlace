namespace Findly.Domain.Entities;

public class Category : Entity
{
    protected Category() { }

    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }
    public bool IsActive { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Category Create(string name, string slug, string? description, string? iconUrl, string createdBy)
    {
        return new Category
        {
            Name = name,
            Slug = slug,
            Description = description,
            IconUrl = iconUrl,
            IsActive = true,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateDetails(string name, string slug, string? description, string? iconUrl, bool isActive, string updatedBy)
    {
        Name = name;
        Slug = slug;
        Description = description;
        IconUrl = iconUrl;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
