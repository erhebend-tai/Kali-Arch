namespace CleanArch.Shared.Abstractions.DTOs.Common;

public sealed record SecretMetadata(
    string Name,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    string Version);
