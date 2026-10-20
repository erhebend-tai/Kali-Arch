namespace CleanArch.Shared.Abstractions.DTOs.Identity;

/// <summary>Access token returned by an identity provider.</summary>
public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAt,
    string? RefreshToken = null,
    string TokenType = "Bearer");
