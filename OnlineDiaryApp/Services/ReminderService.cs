using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Services
{
    public class ReminderService
    {
        private readonly IReminderRepository _reminderRepository;

        public ReminderService(IReminderRepository reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

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
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            return await _reminderRepository.GetAllAsync();
        }

        public async Task<Reminder?> GetReminderByNoteIdAsync(int noteId)
        {
            return await _reminderRepository.GetByNoteIdAsync(noteId);
        }

        public async Task UpdateReminderAsync(Reminder reminder, DateTime? newRemindAt = null, string? newStatus = null)
        {
            if (newRemindAt.HasValue)
                reminder.RemindAt = DateTime.SpecifyKind(newRemindAt.Value, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(newStatus))
                reminder.Status = newStatus;

            await _reminderRepository.UpdateAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
        }

        public async Task DeleteReminderAsync(int reminderId)
        {
            var reminder = await _reminderRepository.GetByIdAsync(reminderId);
            if (reminder != null)
            {
                await _reminderRepository.DeleteAsync(reminder.Id);
                await _reminderRepository.SaveChangesAsync();
            }
        }
    }
}
