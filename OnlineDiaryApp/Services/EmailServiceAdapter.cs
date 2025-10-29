using OnlineDiaryApp.Interfaces;

namespace OnlineDiaryApp.Services
{
    public class EmailServiceAdapter : IEmailSender
    {
        private readonly EmailService _emailService;

        public EmailServiceAdapter(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _emailService.SendEmailAsync(to, subject, body);
        }
    }
}
