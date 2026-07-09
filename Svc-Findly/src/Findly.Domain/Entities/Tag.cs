using Findly.Domain.Enums;

namespace Findly.Domain.Entities;

// A reusable, cross-cutting label shared across Listings
// (e.g. "Slack integration", "Built on React", "SOC-2 certified").
public class Tag : Entity
{
    protected Tag() { }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public TagType Type { get; private set; }

    public static Tag Create(string name, string slug, TagType type, string createdBy)
    {
        return new Tag
        {
            Name = name,
            Slug = slug,
            Type = type,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string name, string slug, TagType type, string updatedBy)
    {
        Name = name;
        Slug = slug;
        Type = type;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
