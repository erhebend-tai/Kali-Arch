using CleanArch.Shared.Abstractions.DTOs.Identity;

namespace CleanArch.Shared.Abstractions.Application.Identity;

/// <summary>
/// External identity provider - Entra ID / Cognito / Keycloak /
/// Auth0 / Firebase Auth.
/// </summary>
public interface IIdentityProviderService
{
    Task<TokenResult> AcquireTokenAsync(
        string scope, CancellationToken ct = default);

    Task<UserInfo> GetUserInfoAsync(
        string userId, CancellationToken ct = default);

    Task<bool> ValidateTokenAsync(
        string token, CancellationToken ct = default);

    Task RevokeTokenAsync(
        string token, CancellationToken ct = default);
}
