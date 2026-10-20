using Microsoft.AspNetCore.Authorization;

namespace CleanArch.Infrastructure.Common.Security.Authorization;

/// <summary>
/// Named policies — referenced in Program.cs via
/// [Authorize(Policy = ...)] or RequireAuthorization.
/// </summary>
public static class AuthorizationPolicies
{
    public const string RequireVmRead = "VmRead";
    public const string RequireVmWrite = "VmWrite";
    public const string RequireAdmin = "Admin";
    public const string RequireComputeQuota = "ComputeQuota";

    public static void Configure(AuthorizationOptions options)
    {
        options.AddPolicy(RequireVmRead, p =>
            p.RequireClaim("scope", "vm.read", "vm.admin"));

        options.AddPolicy(RequireVmWrite, p =>
            p.RequireClaim("scope", "vm.write", "vm.admin"));

        options.AddPolicy(RequireAdmin, p =>
            p.RequireRole("admin"));

        options.AddPolicy(RequireComputeQuota, p =>
            p.AddRequirements(new QuotaRequirement()));
    }
}

/// <summary>
/// Custom requirement — enforces that the caller's quota
/// claim is present and above zero. The handler does the work.
/// </summary>
public sealed class QuotaRequirement : IAuthorizationRequirement;
