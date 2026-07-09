using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.Reviews")]
internal class DbReview
{
    [Key]
    public int          Id                    { get; set; }
    public int          ListingId             { get; set; }
    public int          UserId                { get; set; }
    public string       Title                 { get; set; } = null!;
    public string?      Pros                  { get; set; }
    public string?      Cons                  { get; set; }
    public string?      Comment               { get; set; }
    public decimal      OverallRating         { get; set; }
    public decimal      FeaturesRating        { get; set; }
    public decimal      CustomerSupportRating { get; set; }
    public ReviewStatus Status                { get; set; }
    public string?      RejectionReason       { get; set; }
    public string?      CreatedBy             { get; set; }
    public DateTime     CreatedAt             { get; set; }
    public string?      UpdatedBy             { get; set; }
    public DateTime     UpdatedAt             { get; set; }
}
