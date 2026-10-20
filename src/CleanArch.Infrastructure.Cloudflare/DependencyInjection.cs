using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.Cloudflare;

/// <summary>Registers Cloudflare implementations (R2, Queues, Workers, Tunnels).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCloudflareInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        return services;
    }
}
