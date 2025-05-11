namespace Click_Go.DTOs
{
    public class OpeningHourDto
    {
        // Using string for DayOfWeek initially for flexibility, could use an Enum
        public string DayOfWeek { get; set; }
        public int OpenHour { get; set; }
        public int OpenMinute { get; set; }
        public int CloseHour { get; set; }
        public int CloseMinute { get; set; }
    }
} 