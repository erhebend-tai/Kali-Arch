namespace VirtualMachineService.Application.DTOs.Compute;

public sealed record VmSummaryDto(
    Guid Id,
    string Name,
    string Region,
    string Size,
    string OsType,
    string Status,
    string ResourceGroup,
    DateTime CreatedAt);
