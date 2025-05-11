namespace Click_Go.DTOs
{
    public class CommentDto
    {
        public long PostId { get; set; }
        public long? ParentId { get; set; }
        public string Content { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
