namespace CleanArch.Shared.Abstractions.DTOs.Networking;

/// <summary>An ingress tunnel's identity and endpoint.</summary>
public sealed record TunnelInfo(
    string Id,
    string Name,
    string Hostname,
    TunnelStatus Status,
    DateTime CreatedAt);
