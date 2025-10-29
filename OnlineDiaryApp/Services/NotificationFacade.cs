using OnlineDiaryApp.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class NotificationFacade
    {
        private readonly IEmailSender _emailSender;

        public NotificationFacade(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendEmailNotificationAsync(string to, string subject, string message)
        {
            await _emailSender.SendEmailAsync(to, subject, message);
        }

        public async Task SendNotificationAsync(string to, string subject, string message, string type = "email")
        {
            switch (type.ToLower())
            {
                case "email":
                    await _emailSender.SendEmailAsync(to, subject, message);
                    break;
                default:
                    throw new NotSupportedException($"Тип '{type}' не підтримується.");
            }
        }
    }
}
