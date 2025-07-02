namespace Click_Go.Models
{
    public class Notification : BaseEntity
    {
        public string? SenderId { get; set; }
        public string Message { get; set; }
        public string? Url { get; set; }   
        public bool IsRead { get; set; } = false;
        public long? CommentId { get; set; }
        public Comment? Comment { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
