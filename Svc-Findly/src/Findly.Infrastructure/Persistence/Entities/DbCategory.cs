using Dapper.Contrib.Extensions;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Categories")]
internal class DbCategory
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
