using Microsoft.AspNetCore.Identity;

namespace Click_Go.Models
{
    public class CommentReaction : BaseEntity
    {
        public long? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } 


        public bool? IsLike { get; set; }
    }
}
