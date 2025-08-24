using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KanbanGamev2.Server.Services;

public class VacationBackgroundService : BackgroundService
{
    private readonly ILogger<VacationBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); // Check every minute

    public VacationBackgroundService(
        ILogger<VacationBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Vacation Background Service started. Checking vacation end dates every {Interval} minutes.", _checkInterval.TotalMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();
                
                await employeeService.CheckAndProcessVacationEndsAsync();
                
                _logger.LogDebug("Vacation end date check completed at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking vacation end dates");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Vacation Background Service stopped.");
        await base.StopAsync(cancellationToken);
    }
} 