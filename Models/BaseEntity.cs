using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Click_Go.Models
{
   
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long Id { get; set; }

        [Column("CREATED_USER")]
        public Guid? CreatedUser { get; set; }

        [Column("UPDATED_USER")]
        public Guid? UpdatedUser { get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CreatedDate { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? UpdatedDate { get; set; }

        [Column("STATUS")]
        public int? Status { get; set; }
    }
}
