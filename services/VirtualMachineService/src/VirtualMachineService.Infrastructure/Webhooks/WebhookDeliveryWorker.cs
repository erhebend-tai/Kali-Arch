using System.Text.Json;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VirtualMachineService.Application.Interfaces;

namespace VirtualMachineService.Infrastructure.Webhooks;

/// <summary>
/// Outbox drain for queued webhooks — polls outbox.messages for
/// 'webhook.%' topics and delivers the pre-serialized payload with
/// the same signature path as inline dispatch.
/// </summary>
public sealed class WebhookDeliveryWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<WebhookDeliveryWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval =
        TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await DeliverPendingAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Webhook delivery sweep failed.");
            }

            await Task.Delay(PollInterval, ct);
        }
    }

    private async Task DeliverPendingAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var connectionFactory =
            services.GetRequiredService<IDbConnectionFactory>();
        var webhookService =
            services.GetRequiredService<WebhookService>();
        var registry = services.GetRequiredService<
            Application.Interfaces.Webhooks.IWebhookRegistry>();

        await using var conn = await connectionFactory
            .CreateConnectionAsync(ct);

        var pending = (await conn.QueryAsync<PendingWebhookRow>(
            "SELECT id, payload_type, payload, tenant_id, retry_count " +
            "FROM outbox.messages " +
            "WHERE topic LIKE 'webhook.%' " +
            "  AND processed_at IS NULL " +
            "  AND (next_retry_at IS NULL OR next_retry_at <= NOW()) " +
            "ORDER BY created_at ASC " +
            "LIMIT 50")).ToList();

        foreach (var row in pending)
        {
            try
            {
                var eventType = row.PayloadType
                    .Replace("WebhookEvent:", string.Empty);

                var subscriptions = await registry
                    .GetSubscriptionsAsync(eventType, row.TenantId, ct);

                var tasks = subscriptions
                    .Where(s => s.IsActive)
                    .Select(s => webhookService.DeliverRawAsync(
                        s, row.Id.ToString(), eventType, row.Payload, ct));

                await Task.WhenAll(tasks);

                await conn.ExecuteAsync(
                    "UPDATE outbox.messages " +
                    "SET processed_at = NOW() WHERE id = @id",
                    new { id = row.Id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to dispatch webhook message {Id}", row.Id);

                await conn.ExecuteAsync(
                    "UPDATE outbox.messages " +
                    "SET retry_count   = retry_count + 1, " +
                    "    next_retry_at = NOW() + " +
                    "        (INTERVAL '1 minute' * POWER(2, retry_count)), " +
                    "    error         = @error " +
                    "WHERE id = @id",
                    new { id = row.Id, error = ex.Message });
            }
        }
    }

    private sealed class PendingWebhookRow
    {
        public Guid     Id         { get; init; }
        public string   PayloadType { get; init; } = string.Empty;
        public string   Payload    { get; init; } = string.Empty;
        public string?  TenantId   { get; init; }
        public int      RetryCount { get; init; }
    }
}
