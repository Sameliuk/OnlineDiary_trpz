using OnlineDiaryApp.Patterns.Observers;
using OnlineDiaryApp.Services.Interfaces;

public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private bool _observersAttached = false;

    public ReminderBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
        var emailObserver = scope.ServiceProvider.GetRequiredService<EmailObserver>();
        var logObserver = scope.ServiceProvider.GetRequiredService<LogObserver>();

        reminderService.Attach(emailObserver);
        reminderService.Attach(logObserver);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var nowUtc = DateTime.UtcNow;
                var reminders = (await reminderService.GetAllRemindersAsync())
                    .Where(r => r.Status == "active" && r.RemindAt <= nowUtc)
                    .ToList();

                foreach (var reminder in reminders)
                {
                    await reminderService.NotifyObserversAsync(reminder);
                    await reminderService.UpdateReminderStatusAsync(reminder.Id, "sent");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Reminder service error]: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

}
