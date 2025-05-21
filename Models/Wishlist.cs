using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Click_Go.Models
{
    public class Wishlist
    {
        
        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; } 

        [Required]
        public long PostId { get; set; }
        public virtual Post Post { get; set; } 

        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
} 