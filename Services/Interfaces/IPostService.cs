using Click_Go.DTOs;
using Click_Go.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Click_Go.Services.Interfaces
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(PostCreateDto postDto, string userId);
        Task<GetPostDto> GetPostByIdAsync(long id);
        Task<IEnumerable<GetPostDto>> GetPostsByUserIdAsync(string userId);
        Task<IEnumerable<PostReadDto>> SearchPostsAsync(PostSearchDto searchDto);
        Task<IEnumerable<PostReadDto>> GetAllPostsAsync();
    }
}