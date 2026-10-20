namespace CleanArch.Shared.Abstractions.Application.Secrets;

/// <summary>
/// Automatic secret rotation - decides when a secret is due
/// and performs the rotation through the provider's store.
/// </summary>
public interface ISecretsRotationService
{
    Task RotateAsync(
        string secretName, CancellationToken ct = default);

    Task<bool> IsRotationDueAsync(
        string secretName, CancellationToken ct = default);
}
