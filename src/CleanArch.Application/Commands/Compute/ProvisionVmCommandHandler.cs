using CleanArch.Application.Common;
using CleanArch.Application.Factories.Compute;
using CleanArch.Application.Interfaces;
using CleanArch.Application.Pipelines.Compute;

namespace CleanArch.Application.Commands.Compute;

public sealed class ProvisionVmCommandHandler(
    VmProvisioningContextFactory contextFactory,
    VmProvisioningPipeline pipeline,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ProvisionVmCommand, ProvisionVmResult>
{
    public async Task<ProvisionVmResult> Handle(
        ProvisionVmCommand request,
        CancellationToken ct)
    {
        // Build pipeline context — orchestrates steps,
        // compensates on failure.
        var context = contextFactory.Create(request);
        var result = await pipeline.ExecuteAsync(context, ct);

        if (!result.IsSuccess)
            throw new InvalidOperationException(
                $"Provisioning failed: {result.Error}");

        await unitOfWork.SaveChangesAsync(ct); // UoW commit → domain events dispatch

        var vm = context.CreatedVm!;
        return new ProvisionVmResult(
            vm.Id,
            vm.Name.Value,
            vm.Status.ToString(),
            vm.CreatedAt);
    }
}
