namespace Click_Go.DTOs
{
    public class CommentAncestorPathResult
    {
        public bool Exists { get; set; }
        public List<long> AncestorPath { get; set; } = new();
        public long? RootCommentId { get; set; }
        public int Depth { get; set; }
    }
}
