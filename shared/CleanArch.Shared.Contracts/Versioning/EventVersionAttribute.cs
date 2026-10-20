using System.Reflection;

namespace CleanArch.Shared.Contracts.Versioning;

/// <summary>
/// Tags an integration event contract with its topic + schema
/// version. Consumers resolve the right contract for a message.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class EventVersionAttribute(string topic, int version)
    : Attribute
{
    public string Topic { get; } = topic;
    public int Version { get; } = version;
}

public sealed class EventVersionResolver
{
    private static readonly Dictionary<(string Topic, int Version), Type>
        Registry = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetCustomAttribute<EventVersionAttribute>()
                        is not null)
            .ToDictionary(
                t => (t.GetCustomAttribute<EventVersionAttribute>()!.Topic,
                      t.GetCustomAttribute<EventVersionAttribute>()!.Version),
                t => t);

    public static Type? Resolve(string topic, int version) =>
        Registry.TryGetValue((topic, version), out var type) ? type : null;

    public static int GetLatestVersion(string topic) =>
        Registry.Keys
            .Where(k => k.Topic == topic)
            .Select(k => k.Version)
            .DefaultIfEmpty(0)
            .Max();
}
