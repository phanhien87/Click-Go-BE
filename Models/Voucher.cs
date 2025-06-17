using System.ComponentModel.DataAnnotations;

namespace Click_Go.Models
{
    public class Voucher : BaseEntity
    {
      
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        public double DiscountAmount { get; set; }

        public double? DiscountPercentage { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);

        public bool IsActive { get; set; }

        public int? UsageLimit { get; set; }

        public int? UsedCount { get; set; }

        public long PostId { get; set; } 
        public Post? Post { get; set; }
    }
}
