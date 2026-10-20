using CleanArch.Domain.Entities.Compute;

namespace CleanArch.Domain.Commands.Compute;

public sealed class StopVmDomainCommand(VirtualMachine vm) : IDomainCommand
{
    public string Name => "StopVm";

    public Task ExecuteAsync(CancellationToken ct = default)
    {
        vm.Stop();
        return Task.CompletedTask;
    }
}
