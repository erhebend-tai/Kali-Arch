using System.Text.Json;
using Dapper;
using Microsoft.Extensions.Logging;
using VirtualMachineService.Application.Interfaces;
using VirtualMachineService.Application.Interfaces.Webhooks;
using VirtualMachineService.Application.Webhooks;

namespace VirtualMachineService.Infrastructure.Webhooks;

public sealed class WebhookService(
    IWebhookRegistry registry,
    IWebhookVerifier verifier,
    IDbConnectionFactory connectionFactory,
    HttpClient httpClient,
    ILogger<WebhookService> logger) : IWebhookService
{
    public async Task DispatchAsync<T>(
        string eventType, T payload,
        string? tenantId = null,
        CancellationToken ct = default)
    {
        var subscriptions = await registry.GetSubscriptionsAsync(
            eventType, tenantId, ct);

        var webhookEvent = WebhookEvent<T>.Create(
            eventType, payload, tenantId);

        var json = JsonSerializer.Serialize(webhookEvent);

        var tasks = subscriptions
            .Where(s => s.IsActive)
            .Select(s => DeliverRawAsync(
                s, webhookEvent.EventId.ToString(), eventType, json, ct));

        await Task.WhenAll(tasks);
    }

    public async Task QueueAsync<T>(
        string eventType, T payload,
        string? tenantId = null,
        CancellationToken ct = default)
    {
        // Write to outbox — WebhookDeliveryWorker picks it up.
        // Guarantees at-least-once delivery with retry.
        var webhookEvent = WebhookEvent<T>.Create(
            eventType, payload, tenantId);

        var json = JsonSerializer.Serialize(webhookEvent);

        await using var conn = await connectionFactory
            .CreateConnectionAsync(ct);

        await conn.ExecuteAsync(
            "INSERT INTO outbox.messages " +
            "(topic, payload_type, payload, tenant_id, correlation_id) " +
            "VALUES (@topic, @payloadType, @payload::jsonb, " +
            "        @tenantId, @correlationId)",
            new
            {
                topic         = $"webhook.{eventType}",
                payloadType   = $"WebhookEvent:{eventType}",
                payload       = json,
                tenantId,
                correlationId = webhookEvent.EventId.ToString()
            });
    }

    /// <summary>
    /// Delivers a pre-serialized event — shared by the inline
    /// dispatch path and the outbox retry worker.
    /// </summary>
    internal async Task DeliverRawAsync(
        WebhookSubscription subscription,
        string              eventId,
        string              eventType,
        string              json,
        CancellationToken   ct)
    {
        var signature = verifier.GenerateSignature(json, subscription.Secret);

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            subscription.TargetUrl)
        {
            Content = new StringContent(json,
                System.Text.Encoding.UTF8,
                "application/json")
        };

        // Standard webhook headers
        request.Headers.Add("X-Webhook-ID",        eventId);
        request.Headers.Add("X-Webhook-Timestamp",  DateTime.UtcNow.ToString("O"));
        request.Headers.Add("X-Webhook-Signature",  $"sha256={signature}");
        request.Headers.Add("X-Webhook-Event",      eventType);
        request.Headers.Add("X-Webhook-ApiVersion", "2024-01-01");

        try
        {
            var response = await httpClient.SendAsync(request, ct);

            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation(
                    "Webhook delivered to {Url} for event {EventType}",
                    subscription.TargetUrl, eventType);

                subscription.LastTriggeredAt = DateTime.UtcNow;
                subscription.FailureCount    = 0;
            }
            else
            {
                logger.LogWarning(
                    "Webhook delivery failed. Url={Url}, Status={Status}",
                    subscription.TargetUrl, response.StatusCode);

                subscription.FailureCount++;
            }

            await registry.UpdateAsync(subscription, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Webhook delivery exception for {Url}", subscription.TargetUrl);

            subscription.FailureCount++;
            await registry.UpdateAsync(subscription, ct);
        }
    }
}
