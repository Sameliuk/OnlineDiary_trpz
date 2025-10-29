using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Repositories
{
    public class NotebookRepository : INotebookRepository
    {
        private readonly AppDbContext _context;

        public NotebookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notebook>> GetAllAsync(int userId)
        {
            return await _context.Notebooks
                .Include(n => n.Notes)
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        public async Task<Notebook?> GetByIdAsync(int id)
        {
            return await _context.Notebooks
                .Include(n => n.Notes)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task AddAsync(Notebook notebook)
        {
            await _context.Notebooks.AddAsync(notebook);
        }

        public async Task UpdateAsync(Notebook notebook)
        {
            _context.Notebooks.Update(notebook);
        }

        public async Task DeleteAsync(int id)
        {
            var notebook = await _context.Notebooks.FindAsync(id);
            if (notebook != null)
                _context.Notebooks.Remove(notebook);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
