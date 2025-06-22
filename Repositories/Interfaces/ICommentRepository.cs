using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(long? id);
        Task<IEnumerable<Comment>> GetByPostIdAsync(long postId);
        Task DeleteAsync(long id);
        Task UpdateAsync(Comment comment);

        Task<List<Comment>> getCommentByPost(long postId);
        Task<List<Comment>> GetCommentsByPostAndParent(long postId, long? parentId);
        Task<int> GetReplyCount(long? parentId);
        Task<int> GetTotalCommentByPost(long idPost);
        Task<(bool Success, bool IsRootComment)> DeleteCommentAsync(long commentId, string userId);
        Task<Comment> GetCommentByIdAsync(long commentId);
        Task<List<long>> GetAncestorPathWithCTE(long commentId);
    }
}
