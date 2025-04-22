using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Click_Go.Repositories
{
    public class ReactRepository : IReactRepository
    {
        private readonly ApplicationDbContext _context;

        public ReactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CommentReaction commentReaction)
        {
            
             _context.CommentReactions.Add(commentReaction);
            await _context.SaveChangesAsync();
        }

        public async Task<CommentReaction> GetByCommentIdAndUserId(long commentId, string userId)
        {
            return await _context.CommentReactions.FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == userId);
        }

        public async Task<CommentReaction> GetById(long id)
        {
           return await _context.CommentReactions.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task RemoveAsync(long id)
        {
              var react = await GetById(id);
               _context.CommentReactions.Remove(react);
                await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommentReaction commentReaction)
        {
           _context.CommentReactions.Update(commentReaction);
            await _context.SaveChangesAsync();
        }
    }
}
