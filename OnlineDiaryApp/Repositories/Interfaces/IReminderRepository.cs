using OnlineDiaryApp.Models;

public interface IReminderRepository
{
    Task<IEnumerable<Reminder>> GetAllAsync();
    Task<Reminder?> GetByIdAsync(int id);
    Task<Reminder?> GetByNoteIdAsync(int noteId);
    Task AddAsync(Reminder reminder);
    Task UpdateAsync(Reminder reminder);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
}
