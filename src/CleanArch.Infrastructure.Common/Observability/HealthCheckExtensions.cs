using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArch.Infrastructure.Common.Observability;

/// <summary>
/// Liveness/readiness/startup probes — Kubernetes probes
/// hit these paths.
/// </summary>
public static class HealthCheckExtensions
{
    public static IServiceCollection AddPlatformHealthChecks(
        this IServiceCollection services,
        IConfiguration config)
    {
        var builder = services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(),
                tags: ["live"]);

        var sql = config.GetConnectionString("Database");
        if (sql is not null)
            builder.AddSqlServer(sql, name: "database",
                tags: ["ready"]);

        var redis = config.GetConnectionString("Redis");
        if (redis is not null)
            builder.AddRedis(redis, name: "redis",
                tags: ["ready"]);

        return services;
    }

    public static WebApplication MapPlatformHealthChecks(
        this WebApplication app)
    {
        app.MapHealthChecks("/healthz/live",
            new Microsoft.AspNetCore.Diagnostics.HealthChecks
                .HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("live")
            });

        app.MapHealthChecks("/healthz/ready",
            new Microsoft.AspNetCore.Diagnostics.HealthChecks
                .HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });

        return app;
    }
}
