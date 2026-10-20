using System.Reflection;
using DbUp;
using Microsoft.Extensions.Logging;

namespace VirtualMachineService.Infrastructure.Persistence;

/// <summary>
/// DbUp — plain-SQL migrations embedded in this assembly.
/// EF handles the model; DbUp applies forward-only schema
/// scripts (V-prefixed) in lexical order.
/// </summary>
public static class DbUpMigrator
{
    public static void Migrate(
        string connectionString,
        ILogger? logger = null)
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                Assembly.GetExecutingAssembly())
            .JournalToPostgresqlTable("public", "schema_versions")
            // V05x index scripts use CREATE INDEX CONCURRENTLY,
            // which cannot run inside a transaction.
            .WithoutTransaction()
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
