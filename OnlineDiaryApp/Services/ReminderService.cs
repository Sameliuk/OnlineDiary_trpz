using OnlineDiaryApp.Models;
using OnlineDiaryApp.Interfaces;
using OnlineDiaryApp.Repositories.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class ReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;

        public ReminderService(
            IReminderRepository reminderRepository,
            IUserRepository userRepository,
            IEmailSender emailSender)
        {
            _reminderRepository = reminderRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
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

            await SendReminderEmailAsync(reminder);
        }

        public async Task SendReminderEmailAsync(Reminder reminder)
        {
            var user = reminder.User ?? await _userRepository.GetByIdAsync(reminder.UserId);
            var note = reminder.Note; 

            if (user != null && note != null && !string.IsNullOrEmpty(user.Email))
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    $"Нагадування: {note.Title}",              
                    $"Нагадування по нотатці:\n\n{note.Content}" 
                );
            }
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            return await _reminderRepository.GetAllAsync();
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

        public async Task<Reminder?> GetReminderByNoteIdAsync(int noteId)
        {
            return await _reminderRepository.GetByNoteIdAsync(noteId);
        }

    }
}
