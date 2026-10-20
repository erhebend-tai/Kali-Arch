using System.Text.Json;
using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Services;

/// <summary>
/// Dev/test fallback — logs the payload instead of publishing.
/// Providers replace it with Service Bus / SQS / PubSub etc.
/// </summary>
public sealed class LoggingMessageBusService(
    ILogger<LoggingMessageBusService> logger) : IMessageBusService
{
    public Task PublishAsync<T>(
        string topic, T message, CancellationToken ct = default)
    {
        logger.LogInformation("[Bus → {Topic}] {Payload}",
            topic, JsonSerializer.Serialize(message));
        return Task.CompletedTask;
    }
}
