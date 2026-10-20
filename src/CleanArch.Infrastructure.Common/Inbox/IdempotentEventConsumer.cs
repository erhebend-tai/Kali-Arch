using CleanArch.Application.Interfaces;
using CleanArch.Shared.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Inbox;

/// <summary>
/// Decorator — wraps any event handler with inbox
/// deduplication. Guarantees exactly-once *effect* on top
/// of the outbox's at-least-once delivery.
/// </summary>
public sealed class IdempotentEventConsumer<TEvent>(
    IDomainEventHandler<TEvent> inner,
    IInboxRepository inbox,
    IUnitOfWork uow,
    ILogger<IdempotentEventConsumer<TEvent>> logger)
    : IDomainEventHandler<TEvent>
    where TEvent : IDomainEvent
{
    public async Task HandleAsync(TEvent @event, CancellationToken ct)
    {
        var eventId = $"{typeof(TEvent).Name}:{@event.GetHashCode()}";

        if (await inbox.ExistsAsync(eventId, ct))
        {
            logger.LogDebug(
                "Duplicate event {EventId} — skipped.", eventId);
            return;
        }

        var message = new InboxMessage
        {
            EventId = eventId,
            EventType = typeof(TEvent).Name,
            Payload = System.Text.Json.JsonSerializer.Serialize(@event)
        };

        try
        {
            await inner.HandleAsync(@event, ct);
            message.MarkProcessed();
        }
        catch (Exception ex)
        {
            message.MarkFailed(ex.Message);
            throw;
        }
        finally
        {
            await inbox.AddAsync(message, ct);
            await uow.SaveChangesAsync(ct);
        }
    }
}
