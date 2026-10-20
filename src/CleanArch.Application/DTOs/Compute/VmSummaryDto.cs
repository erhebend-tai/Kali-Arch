namespace CleanArch.Application.DTOs.Compute;

public sealed record VmSummaryDto(
    Guid Id,
    string Name,
    string Region,
    string Size,
    string Status);

public sealed record VmDetailsDto(
    Guid Id,
    string Name,
    string Region,
    string Size,
    string OsType,
    string Status,
    string ResourceGroup,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ProvisionVmResponse(
    Guid Id,
    string Name,
    string Status);
