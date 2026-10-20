namespace CleanArch.Shared.Extensions;

public static class SpanExtensions
{
    /// <summary>Zero-allocation token split.</summary>
    public static int SplitToSpans(
        this ReadOnlySpan<char> source,
        char separator,
        Span<Range> destinations)
    {
        var count = 0;
        var start = 0;

        for (var i = 0; i <= source.Length; i++)
        {
            if (i != source.Length && source[i] != separator) continue;

            if (count >= destinations.Length) return -1;

            destinations[count++] = new Range(start, i);
            start = i + 1;
        }

        return count;
    }

    public static bool IsNullOrEmpty(this string? value) =>
        string.IsNullOrEmpty(value);
}
