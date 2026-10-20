using Microsoft.Extensions.Configuration;

namespace CleanArch.Shared.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Required configuration — throws at startup instead of
    /// silently defaulting mid-request.
    /// </summary>
    public static string GetRequiredValue(
        this IConfiguration configuration, string key) =>
        configuration[key]
        ?? throw new InvalidOperationException(
            $"Required configuration value '{key}' is missing.");

    public static T GetRequiredSection<T>(
        this IConfiguration configuration, string sectionName)
        where T : new()
    {
        var section = configuration.GetSection(sectionName);
        if (!section.Exists())
            throw new InvalidOperationException(
                $"Required configuration section '{sectionName}' " +
                "is missing.");

        return section.Get<T>()!;
    }

    public static bool IsSet(
        this IConfiguration configuration, string key) =>
        !string.IsNullOrEmpty(configuration[key]);
}
