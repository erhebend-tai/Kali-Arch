using CleanArch.FileWatcher.Handlers;
using CleanArch.FileWatcher.Watchers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CleanArch.FileWatcher.Extensions;

public static class FileWatcherExtensions
{
    /// <summary>
    /// Reads watchers/config/watchers.json — each entry becomes a
    /// hosted ConfigFileWatcher with matching handlers.
    /// </summary>
    public static IServiceCollection AddFileWatchers(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IFileChangedHandler, BlobSyncHandler>(
            sp => ActivatorUtilities.CreateInstance<BlobSyncHandler>(
                sp,
                configuration.GetValue<string>(
                    "Watchers:Container") ?? "content"));

        var path = configuration.GetValue<string>(
            "Watchers:Path") ?? "./watched";
        var filter = configuration.GetValue<string>(
            "Watchers:Filter") ?? "*.*";

        services.AddSingleton<IHostedService>(sp =>
            ActivatorUtilities.CreateInstance<ConfigFileWatcher>(
                sp, path, filter));

        return services;
    }
}
