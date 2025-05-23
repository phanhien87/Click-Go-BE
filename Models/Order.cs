using Click_Go.Enum;

namespace Click_Go.Models
{
    public class Order : BaseEntity
    {
        public string OrderCode { get; set; } 
        public string UserId { get; set; }      
        public long PackageId { get; set; }    
        public long Amount { get; set; }   
        public OrderStatus Status { get; set; } = OrderStatus.Pending; 
        public string? transactionDateTime { get; set; }
        public string? TransactionId { get; set; }
        public ApplicationUser User { get; set; }
        public Package Package { get; set; }
        public string? counterAccountBankName { get; set; }
        public string? counterAccountName { get; set; }
        public string? counterAccountNumber { get; set; }
    }
}
