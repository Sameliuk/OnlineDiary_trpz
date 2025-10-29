// Repositories/Interfaces/INoteFileRepository.cs
using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Repositories.Interfaces
{
    public interface INoteFileRepository
    {
        Task AddAsync(NoteFile file);
        Task<IEnumerable<NoteFile>> GetFilesByNoteIdAsync(int noteId);
        Task<NoteFile?> GetByIdAsync(int id);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
