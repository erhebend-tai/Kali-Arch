namespace VirtualMachineService.Application.DTOs.Compute;

public sealed record VmDetailsDto(
    Guid Id,
    string TenantId,
    string Name,
    string Region,
    string Size,
    string OsType,
    string Status,
    string ResourceGroup,
    string Provider,
    string CreatedBy,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
