using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.GCP;

/// <summary>Registers GCP implementations (Cloud Storage, Pub/Sub, Compute Engine).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddGCPInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        return services;
    }
}
