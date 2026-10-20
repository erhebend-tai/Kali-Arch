using System.Text.Json;
using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Common.Outbox;

/// <summary>
/// Decorator — wraps the real bus. Every publish writes an outbox
/// message inside the caller's transaction; OutboxProcessor relays it.
/// Register via Scrutor: services.Decorate&lt;IMessageBusService, OutboxMessageBusDecorator&gt;();
/// </summary>
public sealed class OutboxMessageBusDecorator(
    IMessageBusService inner,
    IOutboxRepository outboxRepository)
    : IMessageBusService
{
    public async Task PublishAsync<T>(
        string topic, T message, CancellationToken ct = default)
    {
        await outboxRepository.AddAsync(new OutboxMessage
        {
            Type = typeof(T).Name,
            Content = JsonSerializer.Serialize(message)
        }, ct);

        // The actual bus call is deferred to OutboxProcessor —
        // inner is still injected so a synchronous relay can be
        // added later without changing call sites.
        _ = inner;
    }
}
