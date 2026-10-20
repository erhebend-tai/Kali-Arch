using CleanArch.Domain.Entities.Compute;

namespace CleanArch.Domain.Commands.Compute;

public sealed class StartVmDomainCommand(VirtualMachine vm) : IDomainCommand
{
    public string Name => "StartVm";

    public Task ExecuteAsync(CancellationToken ct = default)
    {
        vm.Start();
        return Task.CompletedTask;
    }
}
