using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Tags")]
internal class DbTag
{
    [Key]
    public int      Id        { get; set; }
    public string   Name      { get; set; } = null!;
    public string   Slug      { get; set; } = null!;
    public TagType  Type      { get; set; }
    public string?  CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string?  UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
