using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Persistence.Migrations;

/// <summary>
/// DbUp — plain-SQL migrations embedded in this assembly.
/// EF handles the model; DbUp handles forward-only schema
/// scripts (and providers without EF support).
/// </summary>
public static class DbUpMigrator
{
    public static void Migrate(
        string connectionString,
        ILogger? logger = null)
    {
        var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger?.LogError(result.Error,
                "Database migration failed.");
            throw new InvalidOperationException(
                $"Migration failed: {result.Error.Message}",
                result.Error);
        }

        logger?.LogInformation(
            "Database migrations applied successfully.");
    }
}
