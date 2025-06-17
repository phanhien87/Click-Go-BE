using System.ComponentModel.DataAnnotations;
using Click_Go.Models;

namespace Click_Go.DTOs
{
    public class AllVouncherDto
    {
      
        public string Code { get; set; }

        public double DiscountAmount { get; set; }

        public double? DiscountPercentage { get; set; }

        public string? Description { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(7);

        public bool IsActive { get; set; }

        public int? UsageLimit { get; set; }

        public int? UsedCount { get; set; }

    }
}
