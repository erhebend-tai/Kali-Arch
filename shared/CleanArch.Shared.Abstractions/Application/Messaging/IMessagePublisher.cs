using CleanArch.Shared.Domain.Interfaces;

namespace CleanArch.Shared.Abstractions.Application.Messaging;

/// <summary>
/// Outbox-aware publisher - writes to the outbox in the same
/// transaction as business data rather than the bus directly.
/// </summary>
public interface IMessagePublisher
{
    Task PublishAsync<T>(
        string topic, T payload, CancellationToken ct = default);

    Task PublishDomainEventsAsync(
        IEnumerable<IDomainEvent> events,
        CancellationToken ct = default);
}
