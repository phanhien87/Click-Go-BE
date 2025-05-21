namespace Click_Go.DTOs
{
    public class PayOSPaymentWebhookDto
    {
        public string OrderId { get; set; }
        public string PaymentStatus { get; set; }
        public decimal? Amount { get; set; }
        public string TransactionId { get; set; }
        public DateTime? PaymentTime { get; set; }
    }
}
