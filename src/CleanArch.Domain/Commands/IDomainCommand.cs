namespace CleanArch.Domain.Commands;

// GoF Command pattern — encapsulates a domain operation as an object.
// NOTE: different from Application's MediatR ICommand.
public interface IDomainCommand
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken ct = default);
}

public interface IUndoableDomainCommand : IDomainCommand
{
    bool CanUndo { get; }
    Task UndoAsync(CancellationToken ct = default);
}
