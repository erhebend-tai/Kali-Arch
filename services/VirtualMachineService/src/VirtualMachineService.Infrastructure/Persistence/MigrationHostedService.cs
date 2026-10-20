using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VirtualMachineService.Infrastructure.Persistence;

/// <summary>
/// Runs at startup — DbUp applies versioned SQL scripts first,
/// then EF Core migrations bring the EF-managed model in line.
/// Both are opt-out via configuration.
/// </summary>
public sealed class MigrationHostedService(
    IServiceProvider services,
    IConfiguration configuration,
    ILogger<MigrationHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        var connectionString =
            configuration.GetConnectionString("VirtualMachineDb")
            ?? throw new InvalidOperationException(
                "Connection string 'VirtualMachineDb' is missing.");

        if (configuration.GetValue("Database:RunDbUp", true))
        {
            logger.LogInformation("Running DbUp schema scripts...");
            await Task.Run(
                () => DbUpMigrator.Migrate(connectionString, logger), ct);
        }

        if (configuration.GetValue("Database:RunEfMigrations", true))
        {
            logger.LogInformation("Applying EF Core migrations...");
            await using var scope = services.CreateAsyncScope();
            var db = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync(ct);
        }
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
