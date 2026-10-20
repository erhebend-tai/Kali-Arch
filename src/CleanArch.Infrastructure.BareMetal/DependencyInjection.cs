using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.BareMetal;

/// <summary>Registers bare-metal implementations (filesystem storage, SSH/Ansible ops).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBareMetalInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        return services;
    }
}
