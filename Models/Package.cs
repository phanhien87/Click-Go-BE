using System.ComponentModel.DataAnnotations;

namespace Click_Go.Models
{
    public class Package : BaseEntity
    {
       
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int DurationDays { get; set; } // Số ngày duy trì
       
        public long Price { get; set; } // Giá tiền

        

    }
}
