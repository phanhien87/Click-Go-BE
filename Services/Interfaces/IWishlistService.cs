using Click_Go.DTOs;
using Click_Go.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Click_Go.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> ToggleWishlistAsync(string userId, long postId); // Returns true if added, false if removed
        Task<IEnumerable<PostReadDto>> GetUserWishlistAsync(string userId);
    }
} 