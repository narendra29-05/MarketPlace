using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Reviews")]
internal class DbReview
{
    [Key]
    public int          Id                    { get; set; }
    public int          ListingId             { get; set; }
    public string       ReviewerName          { get; set; }
    public string       ReviewerEmail         { get; set; }
    public int          OverallRating         { get; set; }
    public int          FeaturesRating        { get; set; }
    public int          ValueForMoneyRating   { get; set; }
    public int          CustomerSupportRating { get; set; }
    public string       Title                 { get; set; }
    public string       Body                  { get; set; }
    public string?      Pros                  { get; set; }
    public string?      Cons                  { get; set; }
    public ReviewStatus Status                { get; set; }
    public string?      RejectionReason       { get; set; }
    public string?      CreatedBy             { get; set; }
    public DateTime     CreatedAt             { get; set; }
    public string?      UpdatedBy             { get; set; }
    public DateTime     UpdatedAt             { get; set; }
}
