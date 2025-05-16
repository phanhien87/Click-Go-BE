namespace Click_Go.DTOs
{
    public class CreatePaymentLinkDto
    {
        public long packageId { get;set; }
        //public long amount { get; set; }
        public long orderCode { get; set; }
        //public string description { get; set; }
        public string? url { get; set; }
    }
}
