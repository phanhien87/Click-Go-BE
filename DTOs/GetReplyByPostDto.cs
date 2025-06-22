namespace Click_Go.DTOs
{
    public class GetReplyByPostDto
    {
        public long CommentId { get; set; }
        public string Content { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int LikeCount { get; set; }
        public int UnlikeCount { get; set; }
        public List<string> ImagesUrl { get; set; }
        public List<GetCommentByPostDto> Replies { get; set; } = new();
    }
}
