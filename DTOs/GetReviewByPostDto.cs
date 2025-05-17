namespace Click_Go.DTOs
{
    public class GetReviewByPostDto
    {
        public string Content { get; set; }
        public string Username { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public List<RatingDetailDto> Ratings { get; set; } = new();
        public List<IFormFile>? Images { get; set; }
    }
}
