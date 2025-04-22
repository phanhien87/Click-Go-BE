using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace Click_Go.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
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

        public Task<Comment?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async  Task<IEnumerable<Comment>> GetByPostIdAsync(long postId)
        {
            return await _context.Comments
             .Where(c => c.PostId == postId)
             .Include(c => c.User)
             .Include(c => c.Images)
             .Include(c => c.Reactions)
             .ToListAsync();
        }

        public Task UpdateAsync(Comment comment)
        {
            throw new NotImplementedException();
        }
    }
}
