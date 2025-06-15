using Click_Go.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Click_Go.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> SearchTagsByNameAsync(string name);
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> GetTagByIdAsync(long id);
    }
} 