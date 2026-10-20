using CleanArch.Shared.Abstractions.DTOs.Common;

namespace CleanArch.Shared.Abstractions.Application.Secrets;

/// <summary>
/// Cloud-neutral secret store - Key Vault / Secrets Manager /
/// Secret Manager / Vault / file-based.
/// </summary>
public interface ISecretStoreService
{
    Task<string?> GetSecretAsync(
        string name, CancellationToken ct = default);

    Task SetSecretAsync(
        string name, string value, CancellationToken ct = default);

    Task DeleteSecretAsync(
        string name, CancellationToken ct = default);

    Task<SecretMetadata?> GetSecretMetadataAsync(
        string name, CancellationToken ct = default);

    Task RotateSecretAsync(
        string name, CancellationToken ct = default);
}
