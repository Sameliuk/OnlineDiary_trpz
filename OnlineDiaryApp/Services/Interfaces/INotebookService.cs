using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Services.Interfaces
{
    public interface INotebookService
    {
        Task<IEnumerable<Notebook>> GetAllNotebooksAsync(int userId);
        Task<Notebook?> GetNotebookByIdAsync(int id);
        Task<Notebook> CreateNotebookAsync(string name, int userId, string? description = null);
        Task UpdateNotebookAsync(int id, string name, string? description);
        Task DeleteNotebookAsync(int id);
    }
}
