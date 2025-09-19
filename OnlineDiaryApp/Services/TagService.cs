using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class TagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            return await _tagRepository.GetByIdAsync(id);
        }

        public async Task CreateTagAsync(string name)
        {
            var tag = new Tag { Name = name };
            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();
        }

        public async Task DeleteTagAsync(int id)
        {
            await _tagRepository.DeleteAsync(id);
            await _tagRepository.SaveChangesAsync();
        }
    }
}
