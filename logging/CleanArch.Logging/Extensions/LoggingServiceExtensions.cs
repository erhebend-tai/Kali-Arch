using CleanArch.Logging.Configuration;
using CleanArch.Logging.Enrichers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CleanArch.Logging.Extensions;

public static class LoggingServiceExtensions
{
    /// <summary>
    /// One-call Serilog wiring for hosts (API, workers, Functions).
    /// </summary>
    public static IHostBuilder UseCleanArchLogging(
        this IHostBuilder host,
        string serviceName)
    {
        return host.UseSerilog((context, services, loggerConfiguration) =>
        {
            CorrelationIdEnricher.Configure(
                services.GetRequiredService<IHttpContextAccessor>());

            LoggingConfiguration.Create(
                    context.Configuration, serviceName)
                .WriteTo.Console();
        });
    }

    /// <summary>Registers IHttpContextAccessor — required by the correlation enricher.</summary>
    public static IServiceCollection AddCleanArchLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName)
    {
        services.AddHttpContextAccessor();
        return services;
    }
}
