namespace CleanArch.Web.Services;

/// <summary>
/// API client — the web front end calls the same endpoints
/// every other entry point exposes.
/// </summary>
public interface IApiClient
{
    Task<string> GetHealthAsync(CancellationToken ct = default);
    Task<List<VmSummary>> ListVmsAsync(CancellationToken ct = default);
    Task<ProvisionVmResponse?> ProvisionVmAsync(
        ProvisionVmRequest request, CancellationToken ct = default);
}

public sealed record VmSummary(
    string Name,
    string Region,
    string Size,
    string Status);

public sealed record ProvisionVmRequest(
    string Name,
    string ResourceGroup,
    string Region,
    string OsType,
    string Size);

public sealed record ProvisionVmResponse(
    Guid Id,
    string Name,
    string Status);
