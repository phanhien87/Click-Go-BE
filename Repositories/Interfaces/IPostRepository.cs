using Click_Go.DTOs;
using Click_Go.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Click_Go.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<Post> CreateAsync(Post post);
        Task<Post> GetByIdAsync(long id);
        Task<IEnumerable<Post>> GetByUserIdAsync(string userId);
        Task<Category> GetCategoryByIdAsync(long categoryId);
        Task<IEnumerable<Post>> SearchByAddressAsync(string addressQuery);
        Task<IEnumerable<Post>> GetAllAsync();
    }
}
