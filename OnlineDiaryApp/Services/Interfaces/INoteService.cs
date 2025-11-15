using OnlineDiaryApp.Models;
using OnlineDiaryApp.Patterns.Strategy;

namespace OnlineDiaryApp.Services.Interfaces
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetAllNotesByUserAsync(int userId, ISortStrategy? strategy = null);

        Task<IEnumerable<Note>> GetAllNotesAsync(ISortStrategy? strategy = null);

        Task<Note?> GetNoteByIdAsync(int id);

        Task<Note> CreateNoteAsync(string title, string content, int userId, int notebookId, List<int> tagIds);

        Task UpdateNoteAsync(Note note, List<int> tagIds, DateTime? remindAt = null);

        Task DeleteNoteAsync(int id);

        Task<IEnumerable<Note>> SearchByTitleAsync(string keyword);

        Task<IEnumerable<Tag>> GetAllTagsAsync(int userId);

        Task<IEnumerable<Note>> GetNotesByNotebookAsync(int notebookId);
    }
}
