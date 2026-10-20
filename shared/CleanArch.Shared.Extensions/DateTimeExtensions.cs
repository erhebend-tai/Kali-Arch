namespace CleanArch.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime value) =>
        value.Date;

    public static DateTime EndOfDay(this DateTime value) =>
        value.Date.AddDays(1).AddTicks(-1);

    public static DateTime StartOfWeek(
        this DateTime value,
        DayOfWeek firstDay = DayOfWeek.Monday)
    {
        var diff = (7 + (value.DayOfWeek - firstDay)) % 7;
        return value.Date.AddDays(-diff);
    }

    public static bool IsWeekend(this DateTime value) =>
        value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public static long ToUnixTimestamp(this DateTime value) =>
        new DateTimeOffset(value).ToUnixTimeSeconds();

    public static DateTime FromUnixTimestamp(this long timestamp) =>
        DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

    public static string ToRelativeTime(this DateTime value)
    {
        var span = DateTime.UtcNow - value;
        return span.TotalSeconds switch
        {
            < 60 => "just now",
            < 3600 => $"{(int)span.TotalMinutes}m ago",
            < 86400 => $"{(int)span.TotalHours}h ago",
            _ => $"{(int)span.TotalDays}d ago"
        };
    }
}
