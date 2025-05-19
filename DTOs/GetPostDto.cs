using Click_Go.Models;

namespace Click_Go.DTOs
{
    public class GetPostDto
    {
        public PostReadDto Post { get; set; }
        public List<GetCommentByPostDto> Comment { get; set; }
        public OverallCriteriaDto Rating { get; set; }
    }
}
