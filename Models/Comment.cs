using Microsoft.AspNetCore.Identity;

namespace Click_Go.Models
{
    public class Comment : BaseEntity
    {
        public long PostId { get; set; }
        public Post? Post { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; } 


        public string? Content { get; set; }

        public long? ParentId { get; set; }
        public Comment? Parent { get; set; }
        public ICollection<Comment>? Replies { get; set; }

        public ICollection<CommentReaction>? Reactions { get; set; }
        public ICollection<Image>? Images { get; set; }
    }
}
