using Microsoft.AspNetCore.Authorization;

namespace CleanArch.Infrastructure.Common.Security.Authorization;

/// <summary>
/// Checks the quota_remaining claim — provisioning requests
/// are rejected at the pipeline edge, not mid-transaction.
/// </summary>
public sealed class QuotaAuthorizationHandler
    : AuthorizationHandler<QuotaRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        QuotaRequirement requirement)
    {
        var claim = context.User
            .FindFirst("quota_remaining")?.Value;

        if (claim is not null && int.TryParse(claim, out var remaining)
            && remaining > 0)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
