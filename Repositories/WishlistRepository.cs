using Click_Go.Data;
using Click_Go.Models;
using Click_Go.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Click_Go.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {   
            _context = context;
        }

        public async Task AddAsync(Wishlist wishlistItem)
        {   
            await _context.Wishlists.AddAsync(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Wishlist wishlistItem)
        {   
            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync();
        }

        public async Task<Wishlist> GetAsync(string userId, long postId)
        {   
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.PostId == postId);
        }

        public async Task<IEnumerable<Post>> GetUserWishlistPostsAsync(string userId)
        {   
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Post)
                    .ThenInclude(p => p.Category)
                .Include(w => w.Post)
                    .ThenInclude(p => p.User)
                .Include(w => w.Post)
                    .ThenInclude(p => p.Images)
                .Include(w => w.Post)
                    .ThenInclude(p => p.Opening_Hours)
                .Select(w => w.Post)
                .ToListAsync();
        }
    }
} 