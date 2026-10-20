using Microsoft.AspNetCore.Http;

namespace CleanArch.Application.MultiTenancy;

/// <summary>
/// Extracts tenant from JWT claim → subdomain → X-Tenant-ID header,
/// in that order.
/// </summary>
public sealed class TenantMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context, ITenantContext tenantContext)
    {
        var tenantId = context.User.FindFirst("tenant_id")?.Value;

        if (tenantId is null)
        {
            var parts = context.Request.Host.Host.Split('.');
            if (parts.Length > 2)
                tenantId = parts[0];
        }

        tenantId ??= context.Request.Headers["X-Tenant-ID"]
            .FirstOrDefault();

        if (tenantId is null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Tenant ID is required.");
            return;
        }

        tenantContext.SetTenant(tenantId);
        await next(context);
    }
}
