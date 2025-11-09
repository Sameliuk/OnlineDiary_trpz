using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class NotebookService
    {
        private readonly INotebookRepository _notebookRepository;

        public NotebookService(INotebookRepository notebookRepository)
        {
            _notebookRepository = notebookRepository;
        }

        public async Task<IEnumerable<Notebook>> GetAllNotebooksAsync(int userId)
        {
            return await _notebookRepository.GetAllAsync(userId);
        }

        public async Task<Notebook?> GetNotebookByIdAsync(int id)
        {
            return await _notebookRepository.GetByIdAsync(id);
        }


        public async Task<Notebook> CreateNotebookAsync(string name, int userId, string? description = null)
        {
            var notebook = new Notebook
            {
                Name = name,
                Description = description, 
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _notebookRepository.AddAsync(notebook);
            await _notebookRepository.SaveChangesAsync();
            return notebook;
        }

        public async Task UpdateNotebookAsync(int id, string name, string? description)
        {
            var notebook = await _notebookRepository.GetByIdAsync(id);
            if (notebook == null) return;

            notebook.Name = name;
            notebook.Description = description;
            notebook.UpdatedAt = DateTime.UtcNow;

            await _notebookRepository.UpdateAsync(notebook);
            await _notebookRepository.SaveChangesAsync();
        }

        public async Task DeleteNotebookAsync(int id)
        {
            await _notebookRepository.DeleteAsync(id);
            await _notebookRepository.SaveChangesAsync();
        }
    }
}
