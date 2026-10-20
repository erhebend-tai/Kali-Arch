namespace CleanArch.Shared.Domain.Interfaces;

/// <summary>
/// GoF Observer — in-process subscription to a domain event.
/// MediatR INotificationHandler is the usual implementation.
/// </summary>
public interface IDomainEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
