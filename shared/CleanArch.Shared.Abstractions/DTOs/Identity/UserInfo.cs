namespace CleanArch.Shared.Abstractions.DTOs.Identity;

/// <summary>Authenticated principal, normalized across IdPs.</summary>
public sealed record UserInfo(
    string Id,
    string Email,
    string? DisplayName = null,
    IReadOnlyList<string>? Roles = null);
