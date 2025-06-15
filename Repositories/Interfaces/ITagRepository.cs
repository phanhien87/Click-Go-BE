using Click_Go.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Click_Go.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> SearchTagsByNameAsync(string name);
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag> GetByIdAsync(long id);
    }
} 