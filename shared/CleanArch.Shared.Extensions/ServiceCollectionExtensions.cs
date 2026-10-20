using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all concrete implementations of TInterface
    /// found in the given assembly.
    /// </summary>
    public static IServiceCollection AddAllImplementations<TInterface>(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var implementations = assembly.GetTypes()
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && typeof(TInterface).IsAssignableFrom(t));

        foreach (var impl in implementations)
            services.Add(new ServiceDescriptor(
                typeof(TInterface), impl, lifetime));

        return services;
    }

    /// <summary>Conditional registration.</summary>
    public static IServiceCollection AddIf<TService, TImplementation>(
        this IServiceCollection services,
        bool condition,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TService : class
        where TImplementation : class, TService
    {
        if (condition)
            services.Add(new ServiceDescriptor(
                typeof(TService),
                typeof(TImplementation),
                lifetime));

        return services;
    }
}
