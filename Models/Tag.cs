namespace Click_Go.Models
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
