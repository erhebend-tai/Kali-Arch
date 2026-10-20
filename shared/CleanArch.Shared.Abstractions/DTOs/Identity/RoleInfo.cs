namespace CleanArch.Shared.Abstractions.DTOs.Identity;

/// <summary>Role descriptor from an identity provider.</summary>
public sealed record RoleInfo(
    string Id,
    string Name,
    string? Description = null);
