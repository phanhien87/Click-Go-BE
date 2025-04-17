namespace Click_Go.Models
{
    public class RatingDetail : BaseEntity
    {
        public long? RatingId { get; set; }
        public Rating? Rating { get; set; }

        public string? Criteria { get; set; }
        public double? Score { get; set; }
    }
}
