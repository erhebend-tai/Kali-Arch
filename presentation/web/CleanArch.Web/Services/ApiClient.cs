using System.Net.Http.Json;

namespace CleanArch.Web.Services;

public sealed class ApiClient(HttpClient http) : IApiClient
{
    public async Task<string> GetHealthAsync(
        CancellationToken ct = default) =>
        await http.GetStringAsync("/healthz/live", ct);

    public async Task<List<VmSummary>> ListVmsAsync(
        CancellationToken ct = default) =>
        await http.GetFromJsonAsync<List<VmSummary>>(
            "/api/compute/vms", ct) ?? [];

    public async Task<ProvisionVmResponse?> ProvisionVmAsync(
        ProvisionVmRequest request, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(
            "/api/compute/vms", request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content
            .ReadFromJsonAsync<ProvisionVmResponse>(ct);
    }
}
