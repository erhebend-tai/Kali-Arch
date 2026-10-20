using CleanArch.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.Azure;

/// <summary>
/// Registers all Azure implementations of the application contracts.
/// Selected via CloudProvider=Azure in configuration.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAzureInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Provider SDK registrations go here:
        // services.AddScoped<IObjectStorageService, AzureBlobStorageService>();
        // services.AddScoped<IMessageBusService, AzureServiceBusService>();
        // services.AddScoped<IVirtualMachineService, AzureVirtualMachineService>();

        _ = configuration; // bound when SDKs are wired in
        return services;
    }
}
