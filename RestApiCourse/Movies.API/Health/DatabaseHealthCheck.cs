using Microsoft.Extensions.Diagnostics.HealthChecks;
using Movies.Application.Database;

namespace Movies.API.Health
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public const string Name = "DatabaseHealthCheck";
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DatabaseHealthCheck> logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                _ = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection is healthy.");
            }
            catch (Exception ex)
            {
                const string errorMessage = "Database connection check failed.";
                _logger.LogError(ex, errorMessage);

                return HealthCheckResult.Unhealthy(
                    description: errorMessage,
                    exception: ex);
            }
        }
    }
}
