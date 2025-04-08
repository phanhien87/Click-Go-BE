using Microsoft.AspNetCore.Identity;

namespace Click_Go.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
