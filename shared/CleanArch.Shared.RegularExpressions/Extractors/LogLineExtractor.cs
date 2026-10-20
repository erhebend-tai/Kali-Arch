namespace CleanArch.Shared.RegularExpressions.Extractors;

public sealed class LogLineExtractor
{
    public IEnumerable<ParsedLogLine> Extract(string logContent) =>
        Patterns.LogLine().Matches(logContent)
            .Select(m => new ParsedLogLine(
                DateTime.Parse(m.Groups["timestamp"].Value),
                m.Groups["level"].Value,
                m.Groups["message"].Value));
}

public sealed record ParsedLogLine(
    DateTime Timestamp, string Level, string Message);
