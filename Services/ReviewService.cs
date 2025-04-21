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

        public async Task AddReviewAsync(ReviewRequestDto reviewRequestDto,string userId)
        {
            CommentDto commentDto = new CommentDto
            {
                PostId = reviewRequestDto.PostId,
                Content = reviewRequestDto.Content,
                Images = reviewRequestDto.Images,
            };
            await _commentService.AddCommentAsync(commentDto,userId);
            if (reviewRequestDto.Ratings.Any())
            {
                var rating = new Rating
                {
                    PostId = reviewRequestDto.PostId,
                    UserID = userId,
                    CreatedDate = DateTime.Now,
                    Status = 1,
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
        }
    }
}
