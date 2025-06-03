namespace Click_Go.DTOs
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? CheckoutUrl { get; set; }
    }
}
