using OnlineDiaryApp.Models;
using OnlineDiaryApp.Patterns.Observers.Interfaces;

namespace OnlineDiaryApp.Services.Interfaces
{
    public interface IReminderService
    {
        void Attach(IReminderObserver observer);
        void Detach(IReminderObserver observer);

        Task<IEnumerable<Reminder>> GetAllRemindersAsync();

        Task<Reminder?> GetReminderByNoteIdAsync(int noteId);

        Task CreateReminderAsync(int noteId, int userId, DateTime remindAtUtc);

        Task UpdateReminderAsync(int noteId, int userId, DateTime remindAtUtc);

        Task DeleteReminderAsync(int reminderId);

        Task UpdateReminderStatusAsync(int reminderId, string newStatus);

        Task NotifyObserversAsync(Reminder reminder);
    }
}
