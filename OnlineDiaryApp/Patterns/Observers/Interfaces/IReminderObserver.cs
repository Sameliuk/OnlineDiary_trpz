using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Patterns.Observers.Interfaces
{
    public interface IReminderObserver
    {
        Task OnReminderChangedAsync(Reminder reminder, string action);
    }
}
