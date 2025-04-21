using Click_Go.DTOs;

namespace Click_Go.Services.Interfaces
{
    public interface IReviewService
    {
        Task AddReviewAsync(ReviewRequestDto reviewRequestDto, string userId);

    }
}
