using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface ICommentService
    {
        Task AddCommentAsync(CommentDto dto, string userId);
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId);
    }
}
