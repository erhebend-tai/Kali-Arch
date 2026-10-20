namespace CleanArch.Shared.Extensions;

public static class StringExtensions
{
    public static string ToKebabCase(this string value) =>
        string.IsNullOrEmpty(value) ? value :
            System.Text.RegularExpressions.Regex
                .Replace(value, @"([A-Z])", "-$1")
                .TrimStart('-')
                .ToLowerInvariant();

    public static string Truncate(
        this string value, int maxLength, string suffix = "...") =>
        value.Length <= maxLength
            ? value
            : string.Concat(
                value.AsSpan(0, maxLength - suffix.Length), suffix);

    public static bool IsNullOrWhiteSpace(this string? value) =>
        string.IsNullOrWhiteSpace(value);

    public static string ToBase64(this string value) =>
        Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes(value));

    public static string FromBase64(this string value) =>
        System.Text.Encoding.UTF8.GetString(
            Convert.FromBase64String(value));

    public static string MaskSecret(
        this string value, int visibleChars = 4) =>
        value.Length <= visibleChars
            ? new string('*', value.Length)
            : string.Concat(
                new string('*', value.Length - visibleChars),
                value.AsSpan(value.Length - visibleChars));

    public static bool ContainsIgnoreCase(
        this string source, ReadOnlySpan<char> value) =>
        source.AsSpan().Contains(value,
            StringComparison.OrdinalIgnoreCase);
}
