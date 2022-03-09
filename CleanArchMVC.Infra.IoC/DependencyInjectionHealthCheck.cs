using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchMVC.Infra.IoC
{
    public static class DependencyInjectionHealthCheck
    {
        public static IServiceCollection AddInfrastructureHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(
                    connectionString: configuration.GetConnectionString("DefaultConnection"),
                    healthQuery: "SELECT 1;",
                    name: "sqlserver",
                    failureStatus: HealthStatus.Degraded,
                    tags: new string[] { "db", "data", "sqlserver" })
                .AddRedis(
                    "localhost:6379",
                    name: "redis",
                    tags: new string[] { "db", "cache", "redis" });

            return services;
        }
    }
}
