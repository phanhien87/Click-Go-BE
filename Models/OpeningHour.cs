namespace Click_Go.Models
{
    public class OpeningHour : BaseEntity
    {
        public long PostId { get; set; }
        public Post? Post { get; set; }

        public string? DayOfWeek { get; set; }
        public int? OpenHour { get; set; }
        public int? OpenMinute { get; set; }
        public int? CloseHour { get; set; }
        public int? CloseMinute { get; set; }
    }
}
