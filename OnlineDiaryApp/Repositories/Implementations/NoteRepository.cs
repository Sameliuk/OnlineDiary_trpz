using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Repositories.Implementation
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _context;

        public NoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            return await _context.Notes
                .Include(n => n.Tags)
                .Include(n => n.Files)
                .Include(n => n.Reminder)
                .ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _context.Notes
                .Include(n => n.Tags)
                .Include(n => n.Files)
                .Include(n => n.Reminder)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(Note note)
        {
            await _context.Notes.AddAsync(note);
        }

        public async Task UpdateAsync(Note note)
        {
            _context.Notes.Update(note);
        }

        public async Task DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
                _context.Notes.Remove(note);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Note?> GetByIdWithTagsAndFilesAsync(int id)
        {
            return await _context.Notes
                .Include(n => n.Tags)
                .Include(n => n.Files)
                .Include(n => n.Reminder)
                .Include(n => n.Notebook)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        // 🔹 Отримати всі нотатки певного блокнота
        public async Task<IEnumerable<Note>> GetNotesByNotebookIdAsync(int notebookId)
        {
            return await _context.Notes
                .Where(n => n.NotebookId == notebookId)
                .Include(n => n.Tags)
                .Include(n => n.Files)
                .ToListAsync();
        }


        // 🔹 Отримати всі нотатки без блокнота
        public async Task<IEnumerable<Note>> GetNotesWithoutNotebookAsync()
        {
            return await _context.Notes
                .Where(n => n.NotebookId == null)
                .Include(n => n.Tags)
                .Include(n => n.Files)
                .ToListAsync();
        }

        // 🔹 Перемістити нотатку до іншого блокнота
        public async Task MoveNoteToNotebookAsync(int noteId, int notebookId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note != null)
            {
                note.NotebookId = notebookId;
                _context.Notes.Update(note);
                await _context.SaveChangesAsync();
            }
        }

       
        public async Task RemoveNoteFromNotebookAsync(int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);
            if (note == null) return;

            var defaultNotebook = await _context.Notebooks
                .FirstOrDefaultAsync(n => n.UserId == note.UserId && n.Name == "Default");

            if (defaultNotebook == null)
            {
                defaultNotebook = new Notebook
                {
                    Name = "Default",
                    UserId = note.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Notebooks.AddAsync(defaultNotebook);
                await _context.SaveChangesAsync();
            }

            note.NotebookId = defaultNotebook.Id;
            _context.Notes.Update(note);
            await _context.SaveChangesAsync();
        }

    }
}
