namespace CleanArch.Shared.Grpc.Abstractions;

/// <summary>
/// Authenticated principal shape shared by server-side
/// (validate inbound) and client-side (mint outbound) gRPC auth.
/// </summary>
public sealed record UserInfo(
    string UserId,
    string Email,
    IReadOnlyList<string> Roles);
