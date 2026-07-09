using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

public class Category : Entity
{
    protected Category() { }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }

    // Self-reference for sub-categories (e.g. "CRM" under "Sales").
    public int? ParentCategoryId { get; private set; }

    public int DisplayOrder { get; private set; }
    public CategoryStatus Status { get; private set; }

    // =========================================================================
    // Factory
    // =========================================================================

    public static Category Create(
        string name,
        string slug,
        string createdBy,
        string? description = null,
        string? iconUrl = null,
        int? parentCategoryId = null,
        int displayOrder = 0)
    {
        return new Category
        {
            Name = name,
            Slug = slug,
            Description = description,
            IconUrl = iconUrl,
            ParentCategoryId = parentCategoryId,
            DisplayOrder = displayOrder,
            Status = CategoryStatus.Active,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // =========================================================================
    // Domain Methods
    // =========================================================================

    public void UpdateDetails(
        string name,
        string slug,
        string updatedBy,
        string? description,
        string? iconUrl,
        int? parentCategoryId,
        int displayOrder)
    {
        Name = name;
        Slug = slug;
        Description = description;
        IconUrl = iconUrl;
        ParentCategoryId = parentCategoryId;
        DisplayOrder = displayOrder;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Activate(string updatedBy)
    {
        Status = CategoryStatus.Active;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void Deactivate(string updatedBy)
    {
        Status = CategoryStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
