using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface INotebookRepository
    {
        Task<IEnumerable<Notebook>> GetAllAsync(int userId);
        Task<Notebook?> GetByIdAsync(int id);
        Task AddAsync(Notebook notebook);
        Task UpdateAsync(Notebook notebook);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
