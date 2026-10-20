using CleanArch.Shared.Results;
using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.Interfaces;
using VirtualMachineService.Domain.Entities.Compute;
using VirtualMachineService.Domain.Enums.Compute;
using VirtualMachineService.Domain.Exceptions;
using VirtualMachineService.Domain.ValueObjects.Compute;

namespace VirtualMachineService.Application.Commands.Compute;

public sealed class ProvisionVmCommandHandler(
    IVirtualMachineRepository repository)
    : ICommandHandler<ProvisionVmCommand, Result<ProvisionVmResult>>
{
    public async Task<Result<ProvisionVmResult>> Handle(
        ProvisionVmCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<OsType>(request.OsType, ignoreCase: true,
                out var osType))
            return Error.Validation(
                "Vm.InvalidOsType",
                $"'{request.OsType}' is not a valid OS type.");

        try
        {
            var vm = VirtualMachine.Create(
                request.TenantId,
                new ResourceName(request.Name),
                new Region(request.Region),
                new MachineSize(request.Size),
                osType,
                request.ResourceGroup,
                provider: "Azure",
                createdBy: request.RequestedBy);

            // Stored procedure — VM row + outbox message in one transaction.
            await repository.ProvisionAsync(
                vm,
                correlationId: vm.Id.ToString(),
                ct);

            return new ProvisionVmResult(
                vm.Id, vm.Name.Value, vm.Status.ToString().ToLowerInvariant());
        }
        catch (QuotaExceededException ex)
        {
            return Error.Conflict("Vm.QuotaExceeded", ex.Message);
        }
        catch (ResourceAlreadyExistsException ex)
        {
            return Error.Conflict("Vm.Duplicate", ex.Message);
        }
        catch (DomainException ex)
        {
            return Error.Domain("Vm.InvalidState", ex.Message);
        }
    }
}
