namespace CleanArch.Domain.Interfaces;

/// <summary>
/// Domain service contract — quota rules are business logic,
/// so the interface lives in Domain. Implemented in Infrastructure.
/// </summary>
public interface IQuotaService
{
    Task<bool> HasAvailableQuotaAsync(
        string region, string size, CancellationToken ct = default);
}
