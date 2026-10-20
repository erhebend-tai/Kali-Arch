using CleanArch.Application.Common;
using CleanArch.Domain.Commands;
using CleanArch.Domain.Commands.Compute;
using CleanArch.Domain.Interfaces.Repositories.Compute;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Application.Commands.Compute;

public sealed class StartVmCommandHandler(
    IVirtualMachineRepository vmRepository,
    CommandInvoker commandInvoker)
    : ICommandHandler<StartVmCommand>
{
    public async Task Handle(StartVmCommand request, CancellationToken ct)
    {
        var vm = await vmRepository.GetByIdAsync(request.VmId, ct)
            ?? throw new InvalidDomainStateException(
                $"VM {request.VmId} not found.");

        IDomainCommand command = new StartVmDomainCommand(vm);
        await commandInvoker.ExecuteAsync(command, ct);

        await vmRepository.UpdateAsync(vm, ct);
    }
}
