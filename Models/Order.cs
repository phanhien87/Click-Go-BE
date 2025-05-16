using Click_Go.Enum;

namespace Click_Go.Models
{
    public class Order : BaseEntity
    {
        public string OrderCode { get; set; } // Mã đơn hàng duy nhất
        public string UserId { get; set; }       // Id user đặt đơn
        public long PackageId { get; set; }    // Id gói dịch vụ mua
        public long Amount { get; set; }   // Số tiền thanh toán
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Trạng thái đơn hàng (Pending, Paid, Cancelled,...)
        public DateTime? PaymentDate { get; set; }
        public string? TransactionId { get; set; } // nếu PayOS trả về mã giao dịch
        // Navigation properties (nếu có entity User và Package)
        public ApplicationUser User { get; set; }
        public Package Package { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
