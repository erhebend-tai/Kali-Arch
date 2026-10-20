using CleanArch.Application.Common;
using CleanArch.Domain.Commands;
using CleanArch.Domain.Commands.Compute;
using CleanArch.Domain.Interfaces.Repositories.Compute;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Application.Commands.Compute;

public sealed record StopVmCommand(Guid VmId) : ICommand;

public sealed class StopVmCommandHandler(
    IVirtualMachineRepository vmRepository,
    CommandInvoker commandInvoker)
    : ICommandHandler<StopVmCommand>
{
    public async Task Handle(StopVmCommand request, CancellationToken ct)
    {
        var vm = await vmRepository.GetByIdAsync(request.VmId, ct)
            ?? throw new InvalidDomainStateException(
                $"VM {request.VmId} not found.");

        IDomainCommand command = new StopVmDomainCommand(vm);
        await commandInvoker.ExecuteAsync(command, ct);

        await vmRepository.UpdateAsync(vm, ct);
    }
}
