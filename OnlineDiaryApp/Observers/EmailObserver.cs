using OnlineDiaryApp.Interfaces;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;

namespace OnlineDiaryApp.Observers
{
    public class EmailObserver : IReminderObserver
    {
        private readonly NotificationFacade _notificationFacade;

        public EmailObserver(NotificationFacade notificationFacade)
        {
            _notificationFacade = notificationFacade;
        }

        public async Task OnReminderChangedAsync(Reminder reminder, string action)
        {
            if (action == "created" || action == "updated" || action == "time_reached")
            {
                var user = reminder.User;
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string subject = $"Нагадування: {reminder.Note?.Title ?? "Без назви"}";
                    string body = $"Ваше нагадування ({action}).\n\n{reminder.Note?.Content ?? "Без тексту"}";

                    await _notificationFacade.SendEmailNotificationAsync(user.Email, subject, body);
                }
            }
        }
    }
}
