using System.Text.RegularExpressions;

namespace CleanArch.Shared.RegularExpressions;

/// <summary>
/// Source-generated regex — compiled at build time, zero runtime cost.
/// </summary>
public static partial class Patterns
{
    [GeneratedRegex(@"^[a-z0-9][a-z0-9\-]{1,61}[a-z0-9]$",
        RegexOptions.Compiled)]
    public static partial Regex ResourceName();

    [GeneratedRegex(@"^[a-z0-9][a-z0-9\-\.]{1,253}$",
        RegexOptions.Compiled)]
    public static partial Regex DnsName();

    [GeneratedRegex(@"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    public static partial Regex Email();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,}$",
        RegexOptions.Compiled)]
    public static partial Regex StrongPassword();

    [GeneratedRegex(@"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)$",
        RegexOptions.Compiled)]
    public static partial Regex IPv4Address();

    [GeneratedRegex(@"^([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}$",
        RegexOptions.Compiled)]
    public static partial Regex IPv6Address();

    [GeneratedRegex(@"^((25[0-5]|2[0-4]\d|[01]?\d\d?)\.){3}(25[0-5]|2[0-4]\d|[01]?\d\d?)/([0-9]|[1-2]\d|3[0-2])$",
        RegexOptions.Compiled)]
    public static partial Regex CidrBlock();

    [GeneratedRegex(@"(?i)(password|secret|apikey|token|credential)\s*[=:]\s*[""']?[^\s""']{8,}",
        RegexOptions.Compiled)]
    public static partial Regex PotentialSecret();

    [GeneratedRegex(@"^\[(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2})\]\s+(?<level>\w+)\s+(?<message>.+)$",
        RegexOptions.Compiled | RegexOptions.Multiline)]
    public static partial Regex LogLine();

    [GeneratedRegex(@"v?(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)(?:-(?<prerelease>[a-zA-Z0-9.]+))?",
        RegexOptions.Compiled)]
    public static partial Regex SemanticVersion();

    [GeneratedRegex(@"(?i)(server|host|data source)\s*=\s*(?<host>[^;]+)",
        RegexOptions.Compiled)]
    public static partial Regex ConnectionStringHost();
}
