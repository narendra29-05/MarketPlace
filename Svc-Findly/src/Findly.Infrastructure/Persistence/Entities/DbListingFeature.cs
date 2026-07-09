using Dapper.Contrib.Extensions;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.ListingFeatures")]
internal class DbListingFeature
{
    [Key]
    public int      Id            { get; set; }
    public int      ListingId     { get; set; }
    public string   Name          { get; set; } = null!;
    public string?  Description   { get; set; }
    public bool     IsHighlighted { get; set; }
    public string?  CreatedBy     { get; set; }
    public DateTime CreatedAt     { get; set; }
    public string?  UpdatedBy     { get; set; }
    public DateTime UpdatedAt     { get; set; }
}
