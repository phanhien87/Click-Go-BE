using System.ComponentModel.DataAnnotations;

namespace Click_Go.Models
{
    public class Payment : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public long Amount { get; set; }
      
        public string Description { get; set; }
 
        public string ReferenceId { get; set; } // Mã giao dịch PayOS hoặc tương tự

    }
}
