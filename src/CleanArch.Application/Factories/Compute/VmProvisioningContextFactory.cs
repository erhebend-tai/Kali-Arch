using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Interfaces;
using CleanArch.Application.Pipelines.Compute;

namespace CleanArch.Application.Factories.Compute;

/// <summary>
/// Builds the pipeline context from the incoming request —
/// the factory knows how to read the ambient caller.
/// </summary>
public sealed class VmProvisioningContextFactory(
    ICurrentUserService currentUserService)
{
    public VmProvisioningContext Create(ProvisionVmCommand request) =>
        new(request, currentUserService.UserId ?? "anonymous");
}
