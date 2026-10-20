namespace VirtualMachineService.Application.DTOs.Compute;

/// <summary>Read model row from vw_active_vms_by_region.</summary>
public sealed record VmsByRegionDto(
    string Region,
    string Provider,
    int TotalVms,
    int RunningVms,
    int StoppedVms,
    int FailedVms,
    int TenantCount);
