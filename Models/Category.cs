using System.ComponentModel.DataAnnotations;

namespace Click_Go.Models
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }

        public ICollection<Post>? Posts { get; set; }
    }
}
