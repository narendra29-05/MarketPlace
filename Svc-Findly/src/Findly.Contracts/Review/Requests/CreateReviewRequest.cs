namespace Findly.Contracts.Review.Requests;

public class CreateReviewRequest
{
    public int OverallRating { get; set; }
    public int FeaturesRating { get; set; }
    public int ValueForMoneyRating { get; set; }
    public int CustomerSupportRating { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string? Pros { get; set; }
    public string? Cons { get; set; }
}
