using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;

namespace TaskTracker.Services
{
    public class OverdueTaskWorker : BackgroundService                                                      // runs on its own thread separate from the HTTP request pipelines
    {
        private readonly IServiceScopeFactory _scopeFactory;                                                // creates a new scope manually each time the worker runs
        private readonly ILogger<OverdueTaskWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);                                 // controls the logging frequency of the worker, set to check for overdue tasks every minute

        public OverdueTaskWorker(IServiceScopeFactory scopeFactory, ILogger<OverdueTaskWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)                         // CancellationToken is used so the worker stops cleanly when the app shuts down
        {
            _logger.LogInformation("OverdueTaskWorker started at: {time}", DateTimeOffset.Now);

            while(!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForOverdueTasksAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking for overdue tasks at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(_checkInterval, stoppingToken); 
            }
            _logger.LogInformation("OverdueTaskWorker stopped at: {time}", DateTimeOffset.Now);
        }

        private async Task CheckForOverdueTasksAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var overdueTasks = await context.Tasks
                .Where(t => t.DueDate < DateTime.UtcNow && !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();

            if(!overdueTasks.Any())
            {
                _logger.LogInformation("No overdue tasks found at: {time}", DateTimeOffset.Now);
                return;
            }

            _logger.LogWarning("{count} overdue tasks found at: {time}", overdueTasks.Count, DateTimeOffset.Now);

            foreach(var task in overdueTasks)
            {
                var overdueBy = DateTime.UtcNow - task.DueDate;
                _logger.LogWarning("[OVERDUE] Task ID: {id} | Title: {title} | Priority : {priority} | Due Date: {dueDate} | Overdue By: {overdueBy}", 
                    task.Id, 
                    task.Title,
                    task.Priority,
                    task.DueDate.ToString("yyyy-MM-dd HH:mm"),
                    FormatOverdueDuration(overdueBy)
                );
            }
        }

        private string FormatOverdueDuration(TimeSpan overdueBy)
        {
            if (overdueBy.TotalDays >= 1)
                return $"{(int)overdueBy.TotalDays} day(s)";
            if (overdueBy.TotalHours >= 1)
                return $"{(int)overdueBy.TotalHours} hour(s)";
            return $"{(int)overdueBy.TotalMinutes} minute(s)";
        }
    }
}
