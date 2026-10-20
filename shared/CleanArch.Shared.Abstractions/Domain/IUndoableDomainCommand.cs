namespace CleanArch.Shared.Abstractions.Domain;

/// <summary>
/// Reversible domain command - Compensating Transaction pattern.
/// </summary>
public interface IUndoableDomainCommand : IDomainCommand
{
    bool CanUndo { get; }
    Task UndoAsync(CancellationToken ct = default);
}
