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

        public async Task<IEnumerable<Post>> SearchPostsAsync(PostSearchDto searchDto)
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchDto.PostName))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchDto.PostName.ToLower().Trim()));
            }

            // Helper to process address components
            var addressComponents = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchDto.District))
            {
                addressComponents.Add(searchDto.District.ToLower().Trim());
            }
            if (!string.IsNullOrWhiteSpace(searchDto.Ward))
            {
                addressComponents.Add(searchDto.Ward.ToLower().Trim());
            }
            if (!string.IsNullOrWhiteSpace(searchDto.City))
            {
                addressComponents.Add(searchDto.City.ToLower().Trim());
            }

            if (addressComponents.Any())
            {
                query = query.Where(p => p.Address != null && 
                                         addressComponents.All(comp => p.Address.ToLower().Contains(comp)));
            }
            
            return await query
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

        public async Task<UserPackage> GetUserPackageByUserIdAsync(string userId)
        {
            return await _context.UserPackages
                                 .FirstOrDefaultAsync(up => up.UserId == userId);
        }
    }
}
