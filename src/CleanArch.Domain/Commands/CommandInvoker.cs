namespace CleanArch.Domain.Commands;

/// <summary>
/// Executes commands and maintains undo/redo history.
/// </summary>
public sealed class CommandInvoker
{
    private readonly Stack<IUndoableDomainCommand> _undoStack = new();
    private readonly Stack<IUndoableDomainCommand> _redoStack = new();

    public async Task ExecuteAsync(
        IDomainCommand command, CancellationToken ct = default)
    {
        await command.ExecuteAsync(ct);

        if (command is IUndoableDomainCommand undoable)
        {
            _undoStack.Push(undoable);
            _redoStack.Clear(); // new command clears redo history
        }
    }

    public async Task UndoAsync(CancellationToken ct = default)
    {
        if (!_undoStack.TryPop(out var command))
            throw new InvalidOperationException("Nothing to undo.");

        await command.UndoAsync(ct);
        _redoStack.Push(command);
    }

    public async Task RedoAsync(CancellationToken ct = default)
    {
        if (!_redoStack.TryPop(out var command))
            throw new InvalidOperationException("Nothing to redo.");

        await command.ExecuteAsync(ct);
        _undoStack.Push(command);
    }

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public IReadOnlyList<string> History =>
        _undoStack.Select(c => c.Name).ToList();
}
