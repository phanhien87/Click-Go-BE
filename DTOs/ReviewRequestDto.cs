namespace Click_Go.DTOs
{
    public class ReviewRequestDto
    {
        public long PostId { get; set; }
        public string Content { get; set; }

        public List<RatingDetailDto> Ratings { get; set; } = new();
        public List<IFormFile>? Images { get; set; }
    }
}
