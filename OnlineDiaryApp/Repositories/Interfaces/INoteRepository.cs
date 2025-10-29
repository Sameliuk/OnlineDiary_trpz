using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllAsync();
        Task<Note?> GetByIdAsync(int id);
        Task AddAsync(Note note);
        Task UpdateAsync(Note note);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
        Task<Note?> GetByIdWithTagsAndFilesAsync(int id);

        Task<IEnumerable<Note>> GetNotesByNotebookIdAsync(int notebookId);
        Task<IEnumerable<Note>> GetNotesWithoutNotebookAsync();
        Task MoveNoteToNotebookAsync(int noteId, int notebookId);
        Task RemoveNoteFromNotebookAsync(int noteId);

    }
}
