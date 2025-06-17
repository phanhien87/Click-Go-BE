using Click_Go.DTOs;
using Click_Go.Repositories.Interfaces;
using Click_Go.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Click_Go.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<TagDto>> SearchTagsByNameAsync(string name)
        {
            var tags = await _tagRepository.SearchTagsByNameAsync(name);
            return tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            });
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            });
        }

        public async Task<TagDto> GetTagByIdAsync(long id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                return null;
            }
            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name
            };
        }
    }
} 