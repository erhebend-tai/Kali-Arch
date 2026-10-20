using System.Text.Json;

namespace CleanArch.Cli.Output;

public sealed class JsonFormatter : IOutputFormatter
{
    private static readonly JsonSerializerOptions Options =
        new() { WriteIndented = true };

    public void Write<T>(IEnumerable<T> rows) =>
        Console.WriteLine(JsonSerializer.Serialize(rows, Options));

    public void Write<T>(T item) =>
        Console.WriteLine(JsonSerializer.Serialize(item, Options));
}
