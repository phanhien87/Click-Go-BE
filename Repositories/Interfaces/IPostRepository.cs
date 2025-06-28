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
        Task<long?> GetIdPostByUser(string userId);
        Task<IEnumerable<Post>> GetByUserIdAsync(string userId);
        Task<Category> GetCategoryByIdAsync(long categoryId);
        Task<IEnumerable<Post>> SearchPostsAsync(PostSearchDto searchDto);
        Task<int> CountSearchResultsAsync(PostSearchDto searchDto);
        Task<IEnumerable<Post>> GetAllAsync();
        Task<UserPackage> GetUserPackageByUserIdAsync(string userId);
        Task UpdatePostAsync(List<Post> posts);
        Task<Post> UpdatePostAsync(Post post);
        Task<int> GetTotalPostAsync(int? status);
        Task<List<Tag>> GetTagsByIdsAsync(List<long> tagIds);
    }
}
