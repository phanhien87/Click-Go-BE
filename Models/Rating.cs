using Microsoft.AspNetCore.Identity;

namespace Click_Go.Models
{
    public class Rating : BaseEntity
    {
        public string UserID { get; set; }
        public ApplicationUser User { get; set; } 

        public long? PostId { get; set; }
        public Post? Post { get; set; }

        public double? Overall { get; set; }

        public ICollection<RatingDetail>? RatingDetails { get; set; }
    }
}
