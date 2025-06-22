using Click_Go.Models;

namespace Click_Go.DTOs
{
    public class GetCommentByPostDto
    {
        public long CommentId { get; set; }
        public string? Content { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public List<string>? ImagesUrl { get; set; }
        public bool? IsLike { get; set; } = null; 
        public int LikeCount { get; set; }
        public int UnlikeCount { get; set; }
        public int Level { get; set; }
        public List<GetCommentByPostDto> Replies { get; set; } = new();
        public int? ReplyCount { get; set; }

    }
}
