using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.ListingMedia")]
internal class DbListingMedia
{
    [Key]
    public int       Id           { get; set; }
    public int       ListingId    { get; set; }
    public MediaType Type         { get; set; }
    public string    Url          { get; set; } = null!;
    public string?   Caption      { get; set; }
    public int       DisplayOrder { get; set; }
    public string?   CreatedBy    { get; set; }
    public DateTime  CreatedAt    { get; set; }
    public string?   UpdatedBy    { get; set; }
    public DateTime  UpdatedAt    { get; set; }
}
