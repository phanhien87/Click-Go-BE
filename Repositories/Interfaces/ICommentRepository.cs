using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(long id);
        Task<IEnumerable<Comment>> GetByPostIdAsync(long postId);
        Task DeleteAsync(long id);
        Task UpdateAsync(Comment comment);
    }
}
