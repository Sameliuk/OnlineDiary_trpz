using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineDiaryApp.Models;
using OnlineDiaryApp.Services;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ReminderBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();
                var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

                var now = DateTime.UtcNow;

                // Отримуємо всі активні нагадування
                var reminders = (await reminderService.GetAllRemindersAsync())
                                .Where(r => r.Status == "active" && r.RemindAt <= now)
                                .ToList();

                foreach (var reminder in reminders)
                {
                    if (reminder.User?.Email != null && reminder.Note != null)
                    {
                        // Відправка email
                        await emailService.SendEmailAsync(
                            reminder.User.Email,
                            $"Нагадування: {reminder.Note.Title}",
                            $"Нагадування по нотатці:\n\n{reminder.Note.Content}");

                        // Оновлення статусу нагадування
                        await reminderService.UpdateReminderAsync(reminder, newStatus: "sent");
                    }
                }
            }
            catch (Exception ex)
            {
                // Логування помилок
                Console.WriteLine($"Reminder service error: {ex.Message}");
            }

            // Перевіряємо раз на хвилину
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
