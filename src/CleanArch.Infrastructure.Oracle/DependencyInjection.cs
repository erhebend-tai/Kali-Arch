using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.Oracle;

/// <summary>Registers OCI implementations (Object Storage, Streaming, Compute).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddOracleInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        return services;
    }
}
