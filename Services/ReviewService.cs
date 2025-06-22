using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;
using Humanizer;

namespace Click_Go.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ICommentService _commentService;
        private readonly IRatingRepository _ratingRepo;
        private readonly IImageRepository _imageRepo;

        public ReviewService(ICommentService commentService, IRatingRepository ratingRepo, IImageRepository imageRepo)
        {
            _commentService = commentService;
            _ratingRepo = ratingRepo;
            _imageRepo = imageRepo;
        }

        public async Task<ReviewResponseDto> AddReviewAsync(ReviewRequestDto reviewRequestDto,string userId)
        {
            
            CommentDto commentDto = new CommentDto
            {
                PostId = reviewRequestDto.PostId,
                Content = reviewRequestDto.Content,
                Images = reviewRequestDto.Images,
            };
            var comment = await _commentService.AddCommentAsync(commentDto,userId);
            Rating rating = new Rating();
            if (reviewRequestDto.Ratings.Any())
            {
                 rating = new Rating
                {
                    PostId = reviewRequestDto.PostId,
                    UserID = userId,
                    CreatedDate = DateTime.Now,
                    Status = 1,
                    CommentId = comment?.CommentId,
                    Overall = reviewRequestDto.Ratings.Average(x => x.Score),
                    RatingDetails = reviewRequestDto.Ratings.Select(r => new RatingDetail
                    {
                        Criteria = r.Criteria,
                        Score = r.Score,
                        CreatedDate = DateTime.Now,
                        Status = 1
                    }).ToList()
                };

                await _ratingRepo.AddAsync(rating);
               
            }
            return new ReviewResponseDto
            {
                NewReview = comment,
                NewRating = rating.Overall,
            };
        }

        public Task<IEnumerable<GetReviewByPostDto>> GetReviewsByPostAsync(int postId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
