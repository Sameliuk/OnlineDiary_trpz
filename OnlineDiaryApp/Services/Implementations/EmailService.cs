using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using OnlineDiaryApp.Services.Interfaces;
using System.Threading.Tasks;

namespace OnlineDiaryApp.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public EmailService(IConfiguration config)
        {
            _smtpServer = config["SMTP_SERVER"] ?? "smtp.gmail.com";
            _port = int.Parse(config["SMTP_PORT"] ?? "587");
            _username = config["SMTP_USERNAME"] ?? "";
            _password = config["SMTP_PASSWORD"] ?? "";
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Online Diary", _username));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_username, _password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
