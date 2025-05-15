namespace Click_Go.DTOs
{
    public class CreatePaymentLinkDto
    {
       public string? url { get; set; }
       public long amount { get; set; }
        public string description { get; set; }
        public int? level { get; set; }

     

        public long orderCode { get; set; }
    }
}
