namespace CleanArch.Shared.Abstractions.DTOs.Networking;

/// <summary>Parameters for creating an ingress tunnel.</summary>
public sealed record TunnelConfig(
    string Name,
    string Hostname,
    string TargetUrl,
    string? CredentialsPath = null);
