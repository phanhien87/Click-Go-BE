using System;
using System.CodeDom;
using System.Data;
using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Click_Go.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context, DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _context = context;

        }
        public async Task AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool Success, bool IsRootComment, long? newParentId)> DeleteCommentAsync(long commentId, string userId)
        {
            var comment = await GetCommentByIdAsync(commentId);

            if (comment == null)
            {
                return (false, false, null);
            }

            var user = await _context.Users.FindAsync(userId);
            if (comment.UserId != userId)
            {
                return (false, comment.ParentId == null, null);
            }

            // Nếu là comment gốc (ParentId == null), xóa tất cả replies
            if (comment.ParentId == null)
            {
                var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.CommentId == commentId);
                var ratingDetails = await _context.RatingDetails.Where(rd => rd.RatingId == rating.Id).ToListAsync();
                _context.RatingDetails.RemoveRange(ratingDetails);
                _context.Ratings.Remove(rating);
                _context.CommentReactions.RemoveRange(comment.Reactions);

                _context.Images.RemoveRange(comment.Images);

                var replies = await _context.Comments
                                                    .Where(c => c.ParentId == commentId)
                                                    .ToListAsync();

                var childrenCommentIds =  replies.Select(c => c.Id).ToList();

                var childNoti = await _context.Notifications.Where(n => childrenCommentIds.Contains((long)n.CommentId)).ToListAsync();
                _context.Notifications.RemoveRange(childNoti);

                var childImg = await _context.Images.Where(i => childrenCommentIds.Contains((long)i.CommentId)).ToListAsync();
                _context.Images.RemoveRange(childImg);

                var childReact = await _context.CommentReactions.Where(r => childrenCommentIds.Contains((long)r.CommentId)).ToListAsync();
                _context.CommentReactions.RemoveRange(childReact);

                _context.Comments.RemoveRange( await GetAllChildCommentsRecursiveAsync(commentId));

                _context.Comments.Remove(comment);
            }
            else
            {
                // Nếu là reply, cần xử lý các reply con
                var childComments = await _context.Comments
                    .Where(c => c.ParentId == commentId)
                    .ToListAsync();

                if (childComments.Any())
                {
                    // Chuyển các reply con lên cấp cha (gán ParentId của reply bị xóa)
                    foreach (var child in childComments)
                    {
                        child.ParentId = comment.ParentId; // Lên 1 cấp
                        child.UpdatedDate = DateTime.UtcNow;
                    }


                    _context.Comments.UpdateRange(childComments);
                }

                var noti = await _context.Notifications.FirstOrDefaultAsync(r => r.CommentId == commentId);
                if (noti != null) _context.Notifications.Remove(noti);
                // xóa reactions và images của comment hiện tại
                _context.CommentReactions.RemoveRange(comment.Reactions);
                _context.Images.RemoveRange(comment.Images);




                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return (true, comment.ParentId == null, comment.ParentId);
        }

        private async Task<List<Comment>> GetAllChildCommentsRecursiveAsync(long parentId)
        {
            var result = new List<Comment>();

            async Task Traverse(long currentId)
            {
                var children = await _context.Comments
                    .Where(c => c.ParentId == currentId)
                    .OrderBy(c => c.CreatedDate) 
                    .ToListAsync();

                foreach (var child in children)
                {
                    result.Add(child); 
                    await Traverse(child.Id);
                }
            }

            await Traverse(parentId);
            return result;
        }


        public async Task<List<long>> GetAncestorPathWithCTE(long commentId)
        {
            var sql = @"
            WITH CommentHierarchy AS (
                -- Anchor: comment hiện tại
                SELECT Id, ParentId, CAST(Id AS NVARCHAR(MAX)) as Path, 0 as Level
                FROM Comments 
                WHERE Id = @CommentId
                
                UNION ALL
                
                -- Recursive: tìm parent
                SELECT c.Id, c.ParentId, CAST(c.Id AS NVARCHAR(MAX)) + ',' + ch.Path as Path, ch.Level + 1
                FROM Comments c
                INNER JOIN CommentHierarchy ch ON c.Id = ch.ParentId
            )
            SELECT Path, Level
            FROM CommentHierarchy
            WHERE ParentId IS NULL  -- Root comment
            ORDER BY Level DESC";

            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.Parameters.Add(new SqlParameter("@CommentId", commentId));

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var pathString = reader.GetString("Path");
                return pathString.Split(',').Select(long.Parse).ToList();
            }

            // Fallback: nếu CTE không hoạt động, dùng phương pháp đệ quy
            return await GetAncestorPathRecursive(commentId);
        }
        private async Task<List<long>> GetAncestorPathRecursive(long commentId)
        {
            var path = new List<long>();
            long currentId = commentId;

            // Giới hạn độ sâu để tránh infinite loop
            var maxDepth = 50;
            var currentDepth = 0;

            while (currentId != null && currentDepth < maxDepth)
            {
                var comment = await _context.Comments
                    .Where(c => c.Id == currentId)
                    .Select(c => new { c.Id, c.ParentId })
                    .FirstOrDefaultAsync();

                if (comment == null) break;

                path.Insert(0, comment.Id); // Thêm vào đầu để có thứ tự từ root đến leaf

                currentId = (long)comment.ParentId;
                currentDepth++;
            }

            return path;
        }


        public async Task<Comment?> GetByIdAsync(long? id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetByPostIdAsync(long postId)
        {
            return await _context.Comments
             .Where(c => c.PostId == postId)
             .Include(c => c.User)
             .Include(c => c.Images)
             .Include(c => c.Reactions)
             .ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(long commentId)
        {
            return await _context.Comments
                    .Include(c => c.Replies)
                    .Include(c => c.Reactions)
                    .Include(c => c.Images)
                    .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<List<Comment>> getCommentByPost(long postId)
        {
            return await _context.Comments.Include(u => u.User).Include(r => r.Reactions).Include(u => u.Images).Where(p => p.PostId == postId).ToListAsync();
        }

        public async Task<List<Comment>> GetCommentsByPostAndParent(long postId, long? parentId)
        {
            return await _context.Comments
                                        .Where(c => c.PostId == postId && c.ParentId == parentId)
                                        .Include(c => c.User)
                                        .Include(c => c.Images)
                                        .Include(c => c.Reactions)
                                        .OrderBy(c => c.CreatedDate)
                                        .ToListAsync();
        }

        public async Task<int> GetReplyCount(long? parentId)
        {
            //using var context = new ApplicationDbContext(_dbContextOptions);
            return await _context.Comments.CountAsync(r => r.ParentId == parentId);
        }

        public async Task<int> GetTotalCommentByPost(long idPost)
        {
            return await _context.Comments.CountAsync(c => c.PostId == idPost);
        }

        public Task UpdateAsync(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
