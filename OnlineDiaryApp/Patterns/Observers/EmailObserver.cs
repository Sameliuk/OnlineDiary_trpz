using OnlineDiaryApp.Models;
using OnlineDiaryApp.Patterns.Facade;
using OnlineDiaryApp.Patterns.Observers.Interfaces;
using System.Text.RegularExpressions;

namespace OnlineDiaryApp.Patterns.Observers
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
            if (action == "created" || action == "updated")
            {
                var user = reminder.User;
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    string subject = $"Нагадування: {reminder.Note?.Title ?? "Без назви"}";

                    string contentHtml = reminder.Note?.Content ?? "Без тексту";

                    string contentText = Regex.Replace(contentHtml, "<.*?>", string.Empty);

                    contentText = System.Net.WebUtility.HtmlDecode(contentText);

                    string body = $"Ваше нагадування ({action}).\n\n{contentText}";

                    await _notificationFacade.SendEmailNotificationAsync(user.Email, subject, body);
                }
            }
        }
    }
}
