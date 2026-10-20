namespace CleanArch.Shared.Abstractions.DTOs.Compute;

/// <summary>Everything needed to provision a VM on any provider.</summary>
public sealed record VmProvisionRequest(
    string Name,
    string Region,
    string Size,
    string OsType,
    string ResourceGroup,
    string? Image = null,
    IReadOnlyDictionary<string, string>? Tags = null);
