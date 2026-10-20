using CleanArch.Shared.Abstractions.DTOs.Identity;

namespace CleanArch.Shared.Abstractions.Application.Identity;

/// <summary>JWT issue / refresh / validation.</summary>
public interface ITokenService
{
    string GenerateAccessToken(UserInfo user);
    string GenerateRefreshToken();

    Task<UserInfo?> ValidateAccessTokenAsync(
        string token, CancellationToken ct = default);

    bool IsTokenExpired(string token);
}
