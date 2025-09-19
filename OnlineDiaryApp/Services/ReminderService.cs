using OnlineDiaryApp.Models;
using OnlineDiaryApp.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineDiaryApp.Services
{
    public class ReminderService
    {
        private readonly IReminderRepository _reminderRepository;

        public ReminderService(IReminderRepository reminderRepository)
        {
            _reminderRepository = reminderRepository;
        }

        // Створення нового нагадування
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

        // Отримати всі нагадування
        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync()
        {
            return await _reminderRepository.GetAllAsync();
        }

        // Отримати нагадування за NoteId
        public async Task<Reminder?> GetReminderByNoteIdAsync(int noteId)
        {
            return await _reminderRepository.GetByNoteIdAsync(noteId);
        }

        // Оновлення нагадування (дата, статус або обидва)
        public async Task UpdateReminderAsync(Reminder reminder, DateTime? newRemindAt = null, string? newStatus = null)
        {
            if (newRemindAt.HasValue)
                reminder.RemindAt = DateTime.SpecifyKind(newRemindAt.Value, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(newStatus))
                reminder.Status = newStatus;

            await _reminderRepository.UpdateAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
        }

        // Видалити нагадування за Id
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
