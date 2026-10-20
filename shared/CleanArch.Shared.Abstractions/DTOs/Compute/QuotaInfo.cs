namespace CleanArch.Shared.Abstractions.DTOs.Compute;

/// <summary>Capacity quota for a region.</summary>
public sealed record QuotaInfo(
    string Region,
    int TotalVms,
    int UsedVms,
    int AvailableVms,
    IReadOnlyDictionary<string, int>? PerSizeQuota = null);
