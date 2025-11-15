using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync(int userId);

        Task<Tag?> GetTagByIdAsync(int id);

        Task<Tag> CreateTagAsync(string name, int userId);

        Task<IEnumerable<Tag>> GetTagsByUserAsync(int userId);

        Task DeleteTagAsync(int id);
    }
}
