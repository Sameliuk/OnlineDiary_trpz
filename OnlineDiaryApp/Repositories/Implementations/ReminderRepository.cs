using Microsoft.EntityFrameworkCore;
using OnlineDiaryApp.Data;
using OnlineDiaryApp.Models;
public class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reminder>> GetAllAsync()
    {
        return await _context.Reminders
            .Include(r => r.User)   
            .Include(r => r.Note)   
            .ToListAsync();
    }

    public async Task<Reminder?> GetByIdAsync(int id)
    {
        return await _context.Reminders
            .Include(r => r.User)
            .Include(r => r.Note)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Reminder?> GetByNoteIdAsync(int noteId)
    {
        return await _context.Reminders
            .FirstOrDefaultAsync(r => r.NoteId == noteId);
    }

    public async Task AddAsync(Reminder reminder)
    {
        await _context.Reminders.AddAsync(reminder);
    }

    public async Task UpdateAsync(Reminder reminder)
    {
        _context.Reminders.Update(reminder);
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await _context.Reminders.FindAsync(id);
        if (reminder != null)
        {
            _context.Reminders.Remove(reminder);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
