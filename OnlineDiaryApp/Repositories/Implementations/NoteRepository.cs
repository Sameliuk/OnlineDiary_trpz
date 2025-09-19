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
            return await _context.Notes.Include(n => n.Tags).ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(int id)
        {
            return await _context.Notes.Include(n => n.Tags).FirstOrDefaultAsync(n => n.Id == id);
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
    }
}
