using Click_Go.DTOs;
using Click_Go.Models;

namespace Click_Go.Services.Interfaces
{
    public interface ICommentService
    {
        Task<GetReplyByPostDto?> AddCommentAsync(CommentDto dto, string userId);
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(long postId);

        Task<List<GetCommentByPostDto>> GetCommentsByPostAsync(long postId, string? currentUserId = null );

        Task<Comment> GetParentCmtById(long? id);
        Task<List<GetCommentByPostDto>> GetCommentsByPostAndParentAsync(long postId, long? parentId, string? currentUserId = null);
        Task<(bool Success, bool IsRootComment)> DeleteCommentAsync(long commentId, string userId);
        Task<CommentAncestorPathResult> GetCommentAncestorPathAsync(long commentId);
    }
}
