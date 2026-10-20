namespace CleanArch.Shared.Domain.Primitives;

/// <summary>
/// Entity with audit fields — the base type the architecture
/// rules and Unit of Work event dispatch look for.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; protected init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void Touch() => UpdatedAt = DateTime.UtcNow;
}
