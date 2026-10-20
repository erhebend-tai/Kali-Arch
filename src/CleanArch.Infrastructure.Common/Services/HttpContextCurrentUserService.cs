using System.Security.Claims;
using CleanArch.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CleanArch.Infrastructure.Common.Services;

/// <summary>
/// Reads the ambient caller from the HTTP context.
/// Non-HTTP entry points (Functions, workers, MCP) register
/// their own ICurrentUserService sourced from trigger metadata.
/// </summary>
public sealed class HttpContextCurrentUserService(
    IHttpContextAccessor accessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => accessor.HttpContext?.User;

    public string? UserId =>
        User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User?.FindFirstValue("sub");

    public string? UserName =>
        User?.Identity?.Name
        ?? User?.FindFirstValue(ClaimTypes.Name);

    public IReadOnlyList<string> Roles =>
        User?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? [];

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
