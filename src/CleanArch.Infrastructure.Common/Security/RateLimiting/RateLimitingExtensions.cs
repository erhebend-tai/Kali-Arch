using System.Threading.RateLimiting;
using CleanArch.Application.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.Common.Security.RateLimiting;

/// <summary>
/// Built-in .NET rate limiter — global fixed window plus a
/// stricter policy for provisioning endpoints.
/// </summary>
public static class RateLimitingExtensions
{
    public static IServiceCollection AddPlatformRateLimiting(
        this IServiceCollection services, SecurityOptions options)
    {
        services.AddRateLimiter(limiter =>
        {
            limiter.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(
                    context => RateLimitPartition
                        .GetFixedWindowLimiter(
                            context.User.Identity?.Name
                            ?? context.Connection.RemoteIpAddress
                                ?.ToString() ?? "anonymous",
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit =
                                    options.RateLimitPerMinute,
                                Window = TimeSpan.FromMinutes(1)
                            }));

            limiter.AddPolicy("provision", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.User.Identity?.Name ?? "anonymous",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit =
                            options.ProvisionRateLimitPerMinute,
                        Window = TimeSpan.FromMinutes(1)
                    }));

            limiter.RejectionStatusCode = 429;
        });

        return services;
    }
}
