using CleanArch.Shared.Domain.Interfaces;

namespace CleanArch.Shared.Abstractions.Application;

/// <summary>
/// Publishes events to the message bus - arbitrary payloads to a
/// topic, or collected domain events for outbox-aware dispatch.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync<T>(
        string topic, T payload, CancellationToken ct = default);

    Task PublishAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken ct = default);
}
