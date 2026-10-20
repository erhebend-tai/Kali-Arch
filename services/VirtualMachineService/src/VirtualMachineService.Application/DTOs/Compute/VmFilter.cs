namespace VirtualMachineService.Application.DTOs.Compute;

/// <summary>Filter shape passed through to sp_get_vms_by_tenant.</summary>
public sealed record VmFilter(
    string? Status = null,
    string? Region = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20);
