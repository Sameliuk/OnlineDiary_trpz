using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Interfaces
{
    public interface IReminderObserver
    {
        Task OnReminderChangedAsync(Reminder reminder, string action);
    }
}
