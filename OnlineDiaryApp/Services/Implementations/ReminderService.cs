using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services.Interfaces;
using OnlineDiaryApp.Patterns.Observers.Interfaces;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IReminderSubject _reminderSubject;

    public ReminderService(IReminderRepository reminderRepository, IReminderSubject reminderSubject)
    {
        _reminderRepository = reminderRepository;
        _reminderSubject = reminderSubject;
    }

    public void Attach(IReminderObserver observer) => _reminderSubject.Attach(observer);
    public void Detach(IReminderObserver observer) => _reminderSubject.Detach(observer);

    public async Task<Reminder?> GetReminderByNoteIdAsync(int noteId) =>
        await _reminderRepository.GetByNoteIdAsync(noteId);

    public async Task<IEnumerable<Reminder>> GetAllRemindersAsync() =>
        await _reminderRepository.GetAllAsync();

    public async Task CreateReminderAsync(int noteId, int userId, DateTime remindAtUtc)
    {
        var reminder = new Reminder
        {
            NoteId = noteId,
            UserId = userId,
            RemindAt = remindAtUtc,
            Status = "active"
        };

        await _reminderRepository.AddAsync(reminder);
        await _reminderRepository.SaveChangesAsync();

        await _reminderSubject.NotifyObserversAsync(reminder, "created");
    }

    public async Task UpdateReminderAsync(int noteId, int userId, DateTime remindAtUtc)
    {
        var reminder = await _reminderRepository.GetByNoteIdAsync(noteId);

        if (reminder == null)
        {
            reminder = new Reminder
            {
                NoteId = noteId,
                UserId = userId,
                RemindAt = remindAtUtc,
                Status = "active"
            };
            await _reminderRepository.AddAsync(reminder);
            await _reminderSubject.NotifyObserversAsync(reminder, "created");
        }
        else
        {
            reminder.RemindAt = remindAtUtc;
            reminder.Status = "active";
            await _reminderRepository.UpdateAsync(reminder);
            await _reminderSubject.NotifyObserversAsync(reminder, "updated");
        }

        await _reminderRepository.SaveChangesAsync();
    }

    public async Task DeleteReminderAsync(int reminderId)
    {
        var reminder = await _reminderRepository.GetByIdAsync(reminderId);
        if (reminder != null)
        {
            await _reminderRepository.DeleteAsync(reminderId);
            await _reminderRepository.SaveChangesAsync();
            await _reminderSubject.NotifyObserversAsync(reminder, "deleted");
        }
    }

    public async Task UpdateReminderStatusAsync(int reminderId, string newStatus)
    {
        var reminder = await _reminderRepository.GetByIdAsync(reminderId);
        if (reminder != null)
        {
            reminder.Status = newStatus;
            await _reminderRepository.UpdateAsync(reminder);
            await _reminderRepository.SaveChangesAsync();
            await _reminderSubject.NotifyObserversAsync(reminder, "updated");
        }
    }

    public async Task NotifyObserversAsync(Reminder reminder)
    {
        await _reminderSubject.NotifyObserversAsync(reminder, "time_reached");
    }

}
