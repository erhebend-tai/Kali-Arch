namespace CleanArch.Shared.Grpc.Abstractions;

/// <summary>
/// Token issuing/validation abstraction — server interceptors
/// validate inbound tokens, client interceptors mint
/// service-account tokens for outbound calls.
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(UserInfo user);

    Task<UserInfo?> ValidateAccessTokenAsync(
        string token, CancellationToken ct = default);
}
