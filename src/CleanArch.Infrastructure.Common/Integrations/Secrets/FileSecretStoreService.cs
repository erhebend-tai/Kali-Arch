using System.Text.Json;
using CleanArch.Application.Interfaces;

namespace CleanArch.Infrastructure.Common.Integrations.Secrets;

/// <summary>
/// Default secret store — reads secrets/<name>.json from
/// disk. Cloud projects swap this for Key Vault / Secrets
/// Manager / Vault implementations.
/// </summary>
public sealed class FileSecretStoreService(string secretsDirectory)
    : ISecretStoreService
{
    public async Task<string?> GetSecretAsync(
        string name, CancellationToken ct = default)
    {
        var path = PathFor(name);
        if (!File.Exists(path))
            return null;

        var json = await File.ReadAllTextAsync(path, ct);
        return JsonSerializer
            .Deserialize<SecretFile>(json)?.Value;
    }

    public async Task SetSecretAsync(
        string name, string value, CancellationToken ct = default)
    {
        Directory.CreateDirectory(secretsDirectory);
        var file = new SecretFile(
            name, value,
            DateTime.UtcNow, null, Guid.NewGuid().ToString("N")[..8]);
        await File.WriteAllTextAsync(
            PathFor(name), JsonSerializer.Serialize(file), ct);
    }

    public async Task<SecretMetadata?> GetSecretMetadataAsync(
        string name, CancellationToken ct = default)
    {
        var path = PathFor(name);
        if (!File.Exists(path))
            return null;

        var json = await File.ReadAllTextAsync(path, ct);
        var file = JsonSerializer.Deserialize<SecretFile>(json);
        return file is null
            ? null
            : new SecretMetadata(
                file.Name, file.CreatedAt, file.ExpiresAt, file.Version);
    }

    public async Task RotateSecretAsync(
        string name, CancellationToken ct = default)
    {
        var current = await GetSecretAsync(name, ct);
        await SetSecretAsync(
            name,
            $"{current}-rotated-{Guid.NewGuid():N}",
            ct);
    }

    private string PathFor(string name) =>
        Path.Combine(secretsDirectory, $"{name}.json");

    private sealed record SecretFile(
        string Name, string Value,
        DateTime CreatedAt, DateTime? ExpiresAt,
        string Version);
}
