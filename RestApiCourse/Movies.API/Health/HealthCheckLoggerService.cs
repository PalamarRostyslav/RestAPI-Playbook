
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Movies.API.Health
{
    public class HealthCheckLoggerService : BackgroundService
    {
        private readonly ILogger<HealthCheckLoggerService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public HealthCheckLoggerService(ILogger<HealthCheckLoggerService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var healthCheckService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();
                    var report = await healthCheckService.CheckHealthAsync(stoppingToken);

                    _logger.LogInformation("Helth Status: {Status} at time {Time}", report.Status, DateTimeOffset.Now);

                    foreach (var entry in report.Entries)
                    {
                        _logger.LogInformation(" - {Key}: {Status} ({Description})", entry.Key, entry.Value.Status, entry.Value.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing health checks.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
