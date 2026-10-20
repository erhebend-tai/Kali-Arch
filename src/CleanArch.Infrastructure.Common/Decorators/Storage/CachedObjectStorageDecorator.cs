using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Decorators.Storage;

/// <summary>
/// Decorator — adds distributed-cache read-through in front of
/// object storage. Register via Scrutor Decorate().
/// </summary>
public sealed class CachedObjectStorageDecorator(
    IObjectStorageService inner,
    ICacheService cacheService,
    ILogger<CachedObjectStorageDecorator> logger)
    : IObjectStorageService
{
    private static readonly TimeSpan MetadataTtl = TimeSpan.FromMinutes(10);

    public Task<string> UploadAsync(
        string container, string blobName, Stream content,
        string contentType, CancellationToken ct = default) =>
        inner.UploadAsync(container, blobName, content, contentType, ct);

    public async Task<Stream?> DownloadAsync(
        string container, string blobName, CancellationToken ct = default)
    {
        var key = $"blob-meta-{container}-{blobName}";
        var cached = await cacheService.GetAsync<byte[]>(key, ct);
        if (cached is not null)
        {
            logger.LogDebug("Storage cache hit: {Key}", key);
            return new MemoryStream(cached);
        }

        var stream = await inner.DownloadAsync(container, blobName, ct);
        if (stream is null) return null;

        await using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, ct);
        await cacheService.SetAsync(key, ms.ToArray(), MetadataTtl, ct);
        ms.Position = 0;
        return ms;
    }

    public async Task DeleteAsync(
        string container, string blobName, CancellationToken ct = default)
    {
        await inner.DeleteAsync(container, blobName, ct);
        await cacheService.RemoveAsync(
            $"blob-meta-{container}-{blobName}", ct);
    }

    public Task<IReadOnlyList<string>> ListAsync(
        string container, string? prefix = null,
        CancellationToken ct = default) =>
        inner.ListAsync(container, prefix, ct);
}
