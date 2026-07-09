using Dapper.Contrib.Extensions;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.ListingTags")]
internal class DbListingTag
{
    [Key]
    public int      Id        { get; set; }
    public int      ListingId { get; set; }
    public int      TagId     { get; set; }
    public string?  CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string?  UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
