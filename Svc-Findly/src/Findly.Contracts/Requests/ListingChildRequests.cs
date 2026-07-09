using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

// ---- Features ----
public class AddListingFeatureRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsHighlighted { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class UpdateListingFeatureRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsHighlighted { get; set; }
    public string UpdatedBy { get; set; } = null!;
}

// ---- Media ----
public class AddListingMediaRequest
{
    public MediaType Type { get; set; }
    public string Url { get; set; } = null!;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class UpdateListingMediaRequest
{
    public MediaType Type { get; set; }
    public string Url { get; set; } = null!;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public string UpdatedBy { get; set; } = null!;
}

// ---- Category / Tag links ----
public class AddListingCategoryRequest
{
    public int CategoryId { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class AddListingTagRequest
{
    public int TagId { get; set; }
    public string CreatedBy { get; set; } = null!;
}
