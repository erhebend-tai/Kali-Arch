using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Decorators.Storage;

/// <summary>
/// Decorator — timing + structured logging around storage calls.
/// </summary>
public sealed class LoggedObjectStorageDecorator(
    IObjectStorageService inner,
    ILogger<LoggedObjectStorageDecorator> logger)
    : IObjectStorageService
{
    public Task<string> UploadAsync(
        string container, string blobName, Stream content,
        string contentType, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Uploading {BlobName} → {Container}", blobName, container);
        return inner.UploadAsync(
            container, blobName, content, contentType, ct);
    }

    public Task<Stream?> DownloadAsync(
        string container, string blobName, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Downloading {BlobName} ← {Container}", blobName, container);
        return inner.DownloadAsync(container, blobName, ct);
    }

    public Task DeleteAsync(
        string container, string blobName, CancellationToken ct = default)
    {
        logger.LogInformation(
            "Deleting {BlobName} from {Container}", blobName, container);
        return inner.DeleteAsync(container, blobName, ct);
    }

    public Task<IReadOnlyList<string>> ListAsync(
        string container, string? prefix = null,
        CancellationToken ct = default) =>
        inner.ListAsync(container, prefix, ct);
}
