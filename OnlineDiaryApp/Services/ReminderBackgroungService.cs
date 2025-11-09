using OnlineDiaryApp.Patterns.Observers;
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

                reminderService.Attach(scope.ServiceProvider.GetRequiredService<EmailObserver>());
                reminderService.Attach(scope.ServiceProvider.GetRequiredService<LogObserver>());

                var now = DateTime.Now;

                var reminders = (await reminderService.GetAllRemindersAsync())
                    .Where(r => r.Status == "active" && r.RemindAt <= now)
                    .ToList();

                foreach (var reminder in reminders)
                {
                    await reminderService.NotifyObserversAsync(reminder);

                    await reminderService.UpdateReminderAsync(reminder, newStatus: "sent");
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
