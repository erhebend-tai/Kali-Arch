namespace CleanArch.Shared.Abstractions.Application;

/// <summary>
/// Testable clock - inject instead of calling DateTime.UtcNow
/// so time-dependent logic can be frozen in tests.
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
    long UnixTimestampSeconds { get; }
}
