using Click_Go.DTOs;
using Click_Go.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Click_Go.Services.Interfaces
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(PostCreateDto postDto, string userId);
        Task<Post> GetPostByIdAsync(long id);
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId);
    }
} 