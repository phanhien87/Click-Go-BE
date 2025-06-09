using Click_Go.Enum;
using Click_Go.Models;

namespace Click_Go.DTOs
{
    public class OrderCodeDto
    {
        public string OrderCode { get; set; } // Mã đơn hàng duy nhất
        public string UserId { get; set; }       // Id user đặt đơn
        public long PackageId { get; set; }    // Id gói dịch vụ mua
        public long Amount { get; set; }   // Số tiền thanh toán
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Trạng thái đơn hàng (Pending, Paid, Cancelled,...)
        public string? transactionDateTime { get; set; }
        public string? TransactionId { get; set; } // nếu PayOS trả về mã giao dịch
        public Package Package { get; set; }
        public string? counterAccountBankName { get; set; }
        public string? counterAccountName { get; set; }
        public string? counterAccountNumber { get; set; }
    }
} 