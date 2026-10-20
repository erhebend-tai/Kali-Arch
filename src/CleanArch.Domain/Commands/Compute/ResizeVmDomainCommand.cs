using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.ValueObjects.Compute;

namespace CleanArch.Domain.Commands.Compute;

/// <summary>
/// Undoable — captures the previous size so it can be restored.
/// </summary>
public sealed class ResizeVmDomainCommand(
    VirtualMachine vm,
    MachineSize newSize) : IUndoableDomainCommand
{
    private MachineSize? _previousSize;

    public string Name => "ResizeVm";
    public bool CanUndo => _previousSize is not null;

    public Task ExecuteAsync(CancellationToken ct = default)
    {
        _previousSize = vm.Size;
        vm.Resize(newSize);
        return Task.CompletedTask;
    }

    public Task UndoAsync(CancellationToken ct = default)
    {
        if (_previousSize is null)
            throw new InvalidOperationException("Nothing to undo.");

        vm.Resize(_previousSize);
        _previousSize = null;
        return Task.CompletedTask;
    }
}
