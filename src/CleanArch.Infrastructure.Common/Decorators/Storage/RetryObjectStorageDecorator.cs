using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Decorators.Storage;

/// <summary>
/// Decorator — simple fixed-backoff retry around the inner service.
/// </summary>
public sealed class RetryObjectStorageDecorator(
    IObjectStorageService inner,
    ILogger<RetryObjectStorageDecorator> logger,
    int maxAttempts = 3)
    : IObjectStorageService
{
    public async Task<string> UploadAsync(
        string container, string blobName, Stream content,
        string contentType, CancellationToken ct = default) =>
        await RetryAsync(() => inner.UploadAsync(
            container, blobName, content, contentType, ct));

    public async Task<Stream?> DownloadAsync(
        string container, string blobName, CancellationToken ct = default) =>
        await RetryAsync(() => inner.DownloadAsync(container, blobName, ct));

    public async Task DeleteAsync(
        string container, string blobName, CancellationToken ct = default) =>
        await RetryAsync<object?>(async () =>
        {
            await inner.DeleteAsync(container, blobName, ct);
            return null;
        });

    public Task<IReadOnlyList<string>> ListAsync(
        string container, string? prefix = null,
        CancellationToken ct = default) =>
        inner.ListAsync(container, prefix, ct);

    private async Task<T> RetryAsync<T>(Func<Task<T>> operation)
    {
        for (var attempt = 1; ; attempt++)
        {
            try { return await operation(); }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex,
                    "Storage op failed (attempt {Attempt}/{Max})",
                    attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(attempt));
            }
        }
    }
}
