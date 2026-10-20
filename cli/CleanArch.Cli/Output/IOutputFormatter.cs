namespace CleanArch.Cli.Output;

/// <summary>
/// CLI output is pluggable — table for humans, JSON for scripts.
/// </summary>
public interface IOutputFormatter
{
    void Write<T>(IEnumerable<T> rows);
    void Write<T>(T item);
}
