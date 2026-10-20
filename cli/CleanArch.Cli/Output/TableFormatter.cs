namespace CleanArch.Cli.Output;

public sealed class TableFormatter : IOutputFormatter
{
    public void Write<T>(IEnumerable<T> rows)
    {
        foreach (var row in rows)
            Console.WriteLine(row);
    }

    public void Write<T>(T item) => Console.WriteLine(item);
}
