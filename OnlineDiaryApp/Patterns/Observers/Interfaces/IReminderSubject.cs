using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Patterns.Observers.Interfaces
{
    public interface IReminderSubject
    {
        void Attach(IReminderObserver observer);
        void Detach(IReminderObserver observer);
        Task NotifyObserversAsync(Reminder reminder, string action);
    }
}
