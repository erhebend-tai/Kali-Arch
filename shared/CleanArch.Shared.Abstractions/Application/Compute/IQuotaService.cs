using CleanArch.Shared.Abstractions.DTOs.Compute;

namespace CleanArch.Shared.Abstractions.Application.Compute;

/// <summary>
/// Quota enforcement - checked before provisioning so
/// capacity failures surface before any resource is created.
/// </summary>
public interface IQuotaService
{
    Task<bool> HasAvailableQuotaAsync(
        string region, string size, CancellationToken ct = default);

    Task<QuotaInfo> GetQuotaAsync(
        string region, CancellationToken ct = default);

    Task ReserveAsync(
        string region, string size, CancellationToken ct = default);

    Task ReleaseAsync(
        string region, string size, CancellationToken ct = default);
}
