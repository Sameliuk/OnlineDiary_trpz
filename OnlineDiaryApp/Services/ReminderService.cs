using OnlineDiaryApp.Models;
using OnlineDiaryApp.Interfaces;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class ReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReminderSubject _reminderSubject;

        public ReminderService(
            IReminderRepository reminderRepository,
            IUserRepository userRepository,
            IReminderSubject reminderSubject)
        {
            _reminderRepository = reminderRepository;
            _userRepository = userRepository;
            _reminderSubject = reminderSubject;
        }

        public void Attach(IReminderObserver observer) => _reminderSubject.Attach(observer);
        public void Detach(IReminderObserver observer) => _reminderSubject.Detach(observer);

        public async Task CreateReminderAsync(int noteId, DateTime remindAt, int userId)
        {
            var utcRemindAt = DateTime.SpecifyKind(remindAt, DateTimeKind.Utc);

            var reminder = new Reminder
            {
                NoteId = noteId,
                UserId = userId,
                RemindAt = utcRemindAt,
                Status = "active"
            };

            await _reminderRepository.AddAsync(reminder);
            await _reminderRepository.SaveChangesAsync();

            await _reminderSubject.NotifyObserversAsync(reminder, "created");
        }

        public async Task UpdateReminderAsync(Reminder reminder, DateTime? newRemindAt = null, string? newStatus = null)
        {
            if (newRemindAt.HasValue)
                reminder.RemindAt = DateTime.SpecifyKind(newRemindAt.Value, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(newStatus))
                reminder.Status = newStatus;

            await _reminderRepository.UpdateAsync(reminder);
            await _reminderRepository.SaveChangesAsync();

            await _reminderSubject.NotifyObserversAsync(reminder, "updated");
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync() =>
            await _reminderRepository.GetAllAsync();

        public async Task<Reminder?> GetReminderByNoteIdAsync(int noteId) =>
            await _reminderRepository.GetByNoteIdAsync(noteId);

        public async Task DeleteReminderAsync(int reminderId)
        {
            var reminder = await _reminderRepository.GetByIdAsync(reminderId);
            if (reminder != null)
            {
                await _reminderRepository.DeleteAsync(reminder.Id);
                await _reminderRepository.SaveChangesAsync();

                await _reminderSubject.NotifyObserversAsync(reminder, "deleted");
            }
        }

        public async Task NotifyObserversAsync(Reminder reminder)
        {
            await _reminderSubject.NotifyObserversAsync(reminder, "time_reached");
        }
    }
}
