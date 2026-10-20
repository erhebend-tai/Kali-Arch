using CleanArch.Shared.Domain.Interfaces;

namespace CleanArch.Shared.Domain.Primitives;

/// <summary>
/// Base entity — identity + domain event collection.
/// All domain events dispatch before persistence commits.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected init; } = Guid.NewGuid();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
