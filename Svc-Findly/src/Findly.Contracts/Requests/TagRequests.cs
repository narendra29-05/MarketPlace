using Findly.Domain.Enums;

namespace Findly.Contracts.Requests;

public class CreateTagRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public TagType Type { get; set; }
    public string CreatedBy { get; set; } = null!;
}

public class UpdateTagRequest
{
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public TagType Type { get; set; }
    public string UpdatedBy { get; set; } = null!;
}
