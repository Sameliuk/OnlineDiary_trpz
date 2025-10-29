using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync(int userId);
        Task<Tag?> GetByIdAsync(int id);
        Task AddAsync(Tag tag);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}



