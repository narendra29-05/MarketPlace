using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Categories")]
internal class DbCategory
{
    [Key]
    public int            Id               { get; set; }
    public string         Name             { get; set; } = null!;
    public string         Slug             { get; set; } = null!;
    public string?        Description      { get; set; }
    public string?        IconUrl          { get; set; }
    public int?           ParentCategoryId { get; set; }
    public int            DisplayOrder     { get; set; }
    public CategoryStatus Status           { get; set; }
    public string?        CreatedBy        { get; set; }
    public DateTime       CreatedAt        { get; set; }
    public string?        UpdatedBy        { get; set; }
    public DateTime       UpdatedAt        { get; set; }
}
