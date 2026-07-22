namespace Findly.Contracts.Category.Responses;

public class CategoryResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }

}
