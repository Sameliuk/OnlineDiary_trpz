using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Repositories
{
    public class NoteFileRepository : INoteFileRepository
    {
        private readonly AppDbContext _context;

        public NoteFileRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NoteFile file)
        {
            await _context.NoteFiles.AddAsync(file);
        }

        public async Task<IEnumerable<NoteFile>> GetFilesByNoteIdAsync(int noteId)
        {
            return await _context.NoteFiles
                .Where(f => f.NoteId == noteId)
                .ToListAsync();
        }

        public async Task<NoteFile?> GetByIdAsync(int id)
        {
            return await _context.NoteFiles.FindAsync(id);
        }

        public async Task DeleteAsync(int id)
        {
            var file = await GetByIdAsync(id);
            if (file != null)
            {
                // Видалення фізичного файлу
                if (!string.IsNullOrEmpty(file.FileName))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles", file.FileName);
                    if (File.Exists(path))
                        File.Delete(path);
                }

                _context.NoteFiles.Remove(file);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
