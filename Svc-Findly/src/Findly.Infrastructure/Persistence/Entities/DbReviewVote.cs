using Dapper.Contrib.Extensions;
using Findly.Domain.Enums;

namespace Findly.Infrastructure.Persistence.Entities;

[Table("fin.ReviewVotes")]
internal class DbReviewVote
{
    [Key]
    public int      Id        { get; set; }
    public int      ReviewId  { get; set; }
    public int      UserId    { get; set; }
    public VoteType Vote      { get; set; }
    public string?  CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string?  UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
