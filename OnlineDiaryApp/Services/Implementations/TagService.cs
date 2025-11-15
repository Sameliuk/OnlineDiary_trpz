using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;
using OnlineDiaryApp.Services.Interfaces;

namespace OnlineDiaryApp.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync(int userId)
        {
            var allTags = await _tagRepository.GetAllAsync(userId);
            return allTags.Where(t => t.UserId == userId);
        }

        public async Task<Tag?> GetTagByIdAsync(int id) =>
            await _tagRepository.GetByIdAsync(id);

        public async Task<Tag> CreateTagAsync(string name, int userId)
        {
            var tag = new Tag
            {
                Name = name,
                UserId = userId
            };

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            return tag;
        }

        public async Task<IEnumerable<Tag>> GetTagsByUserAsync(int userId)
        {
            var tags = await _tagRepository.GetAllAsync(userId);
            return tags.Where(t => t.UserId == userId);
        }

        public async Task DeleteTagAsync(int id)
        {
            await _tagRepository.DeleteAsync(id);
            await _tagRepository.SaveChangesAsync();
        }
    }
}
