namespace CleanArch.Shared.RegularExpressions.Validators;

public static class ConnectionStringValidator
{
    /// <summary>Extracts the host part of a connection string.</summary>
    public static string? ExtractHost(string connectionString)
    {
        var match = Patterns.ConnectionStringHost().Match(connectionString);
        return match.Success ? match.Groups["host"].Value : null;
    }
}
