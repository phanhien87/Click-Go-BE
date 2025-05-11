using Click_Go.Models;

namespace Click_Go.Repositories.Interfaces
{
    public interface IReactRepository
    {
        Task AddAsync(CommentReaction commentReaction);
        Task RemoveAsync(long id);
        Task UpdateAsync(CommentReaction commentReaction);
        Task<CommentReaction> GetById(long id);
        Task<CommentReaction> GetByCommentIdAndUserId(long commentId,string userId);
    }
}
