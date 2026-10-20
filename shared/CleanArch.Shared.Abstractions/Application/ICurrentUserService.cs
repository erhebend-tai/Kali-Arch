namespace CleanArch.Shared.Abstractions.Application;

/// <summary>
/// Who is calling - filled from HttpContext / Dapr metadata /
/// event payload by each entry point.
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    string? TenantId { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsInRole(string role);
}
