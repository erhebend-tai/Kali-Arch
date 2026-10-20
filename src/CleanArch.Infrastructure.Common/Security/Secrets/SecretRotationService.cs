using CleanArch.Application.Interfaces;
using CleanArch.Application.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArch.Infrastructure.Common.Security.Secrets;

/// <summary>
/// Background service — rotates secrets nearing expiry
/// through the provider's ISecretStoreService.
/// </summary>
public sealed class SecretRotationService(
    ISecretStoreService secretStore,
    IOptions<SecurityOptions> options,
    ILogger<SecretRotationService> logger)
    : BackgroundService
{
    private static readonly string[] ManagedSecrets =
        ["db-connection", "message-bus-key", "jwt-signing-key"];

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var name in ManagedSecrets)
            {
                var metadata =
                    await secretStore.GetSecretMetadataAsync(
                        name, stoppingToken);

                if (metadata?.ExpiresAt is not { } expiresAt)
                    continue;

                var daysUntilExpiry =
                    (expiresAt - DateTime.UtcNow).TotalDays;

                if (daysUntilExpiry <=
                    options.Value.SecretRotationBufferDays)
                {
                    logger.LogInformation(
                        "Rotating secret {Name} — expires in {Days:F0} days.",
                        name, daysUntilExpiry);

                    await secretStore.RotateSecretAsync(
                        name, stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        }
    }
}
