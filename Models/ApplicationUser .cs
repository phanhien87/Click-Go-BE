using Microsoft.AspNetCore.Identity;

namespace Click_Go.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public Post Post { get; set; }
       
        public int? Level { get; set; } = 0;

    }
}
