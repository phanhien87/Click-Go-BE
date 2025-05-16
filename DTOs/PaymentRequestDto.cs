namespace Click_Go.DTOs
{
    public class PaymentRequestDto
    {
        public long PackageId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
