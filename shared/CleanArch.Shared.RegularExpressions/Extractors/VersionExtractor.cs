namespace CleanArch.Shared.RegularExpressions.Extractors;

public sealed class VersionExtractor
{
    public SemanticVersion? Extract(string text)
    {
        var match = Patterns.SemanticVersion().Match(text);
        if (!match.Success) return null;

        return new SemanticVersion(
            int.Parse(match.Groups["major"].Value),
            int.Parse(match.Groups["minor"].Value),
            int.Parse(match.Groups["patch"].Value),
            match.Groups["prerelease"].Success
                ? match.Groups["prerelease"].Value
                : null);
    }
}

public sealed record SemanticVersion(
    int Major, int Minor, int Patch, string? Prerelease);
