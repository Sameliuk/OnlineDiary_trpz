using OnlineDiaryApp.Interfaces;
using OnlineDiaryApp.Models;

namespace OnlineDiaryApp.Observers
{
    public class ReminderSubject : IReminderSubject
    {
        private readonly List<IReminderObserver> _observers = new();

        public void Attach(IReminderObserver observer) => _observers.Add(observer);
        public void Detach(IReminderObserver observer) => _observers.Remove(observer);

        public async Task NotifyObserversAsync(Reminder reminder, string action)
        {
            foreach (var observer in _observers)
                await observer.OnReminderChangedAsync(reminder, action);
        }
    }
}
