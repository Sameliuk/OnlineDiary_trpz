using OnlineDiaryApp.Models;
using OnlineDiaryApp.Patterns.Observers.Interfaces;

namespace OnlineDiaryApp.Patterns.Observers
{
    public class LogObserver : IReminderObserver
    {
        public Task OnReminderChangedAsync(Reminder reminder, string action)
        {
            Console.WriteLine($"[{DateTime.Now}] Reminder {action.ToUpper()} → ID={reminder.Id}, NoteId={reminder.NoteId}, UserId={reminder.UserId}, Status={reminder.Status}");
            return Task.CompletedTask;
        }
    }
}
