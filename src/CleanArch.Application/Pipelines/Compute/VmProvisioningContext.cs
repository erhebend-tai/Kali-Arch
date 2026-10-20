using CleanArch.Application.Commands.Compute;
using CleanArch.Domain.Entities.Compute;

namespace CleanArch.Application.Pipelines.Compute;

/// <summary>
/// Context flowing through the VM provisioning pipeline.
/// Each step reads inputs and deposits outputs here.
/// </summary>
public sealed class VmProvisioningContext(
    ProvisionVmCommand request,
    string userId) : PipelineContext
{
    public ProvisionVmCommand Request { get; } = request;
    public string UserId { get; } = userId;

    // Populated by steps
    public VirtualMachine? CreatedVm { get; set; }
    public string? ReservationId { get; set; }
    public string? NetworkInterfaceId { get; set; }
    public string? AgentInstallId { get; set; }
}
