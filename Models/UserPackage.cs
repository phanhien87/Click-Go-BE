using System.ComponentModel.DataAnnotations.Schema;

namespace Click_Go.Models
{
    public class UserPackage : BaseEntity
    {

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
      
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpireDate { get; set; }
        public long OrderId { get; set; }
        public Order Order { get; set; }
    }
}
