using Dapper.Contrib.Extensions;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Bookmarks")]
internal class DbBookmark
{
    [Key]
    public int      Id        { get; set; }
    public int      UserId    { get; set; }
    public int      ListingId { get; set; }
    public string?  CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string?  UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
