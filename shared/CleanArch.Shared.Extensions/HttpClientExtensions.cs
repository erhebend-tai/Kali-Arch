using System.Net.Http.Json;
using System.Text.Json;

namespace CleanArch.Shared.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T?> GetJsonAsync<T>(
        this HttpClient client,
        string url,
        CancellationToken ct = default) =>
        await client.GetFromJsonAsync<T>(url, ct);

    public static async Task<T?> PostJsonAsync<T>(
        this HttpClient client,
        string url,
        object body,
        CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync(url, body, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ct);
    }

    public static async Task<T?> SendJsonAsync<T>(
        this HttpClient client,
        HttpMethod method,
        string url,
        object? body = null,
        CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
            request.Content = JsonContent.Create(body);

        var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ct);
    }
}
