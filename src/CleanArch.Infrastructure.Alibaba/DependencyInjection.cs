using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.Alibaba;

/// <summary>Registers Alibaba implementations (OSS, MNS, ECS).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAlibabaInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        return services;
    }
}
