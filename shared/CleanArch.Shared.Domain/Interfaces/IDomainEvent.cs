namespace CleanArch.Shared.Domain.Interfaces;

/// <summary>
/// Domain event — raised by an entity, handled in-process before
/// the transaction commits. Never crosses a service boundary
/// (integration events in Shared.Contracts do that).
/// </summary>
public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}
