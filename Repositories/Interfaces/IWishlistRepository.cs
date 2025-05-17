using Click_Go.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Click_Go.Repositories.Interfaces
{
    public interface IWishlistRepository
    {
        Task AddAsync(Wishlist wishlistItem);
        Task RemoveAsync(Wishlist wishlistItem);
        Task<Wishlist> GetAsync(string userId, long postId);
        Task<IEnumerable<Post>> GetUserWishlistPostsAsync(string userId);
    }
} 