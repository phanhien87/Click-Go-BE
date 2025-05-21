using Click_Go.Data;
using Click_Go.DTOs;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Click_Go.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> GetByIdAsync(long id)
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetByUserIdAsync(string userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(long categoryId)
        {
            return await _context.Categories.FindAsync(categoryId);
        }

        public async Task<IEnumerable<Post>> SearchByAddressAsync(string addressQuery)
        {
            if (string.IsNullOrWhiteSpace(addressQuery))
            {
                return Enumerable.Empty<Post>(); 
            }

            var lowerCaseQuery = addressQuery.ToLower().Trim(); 

            return await _context.Posts
                .Where(p => p.Address != null && p.Address.ToLower().Contains(lowerCaseQuery))
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Images)
                .Include(p => p.Opening_Hours)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.User)
                .Include(p => p.Opening_Hours)
                .Include(p => p.Images)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }
    }
}
