using CleanArch.Shared.Domain.Interfaces;

namespace CleanArch.Domain.Events.Compute;

public sealed record VmStartedEvent(Guid VmId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
