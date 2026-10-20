namespace CleanArch.Application.Interfaces.Reports;

public interface IReportGenerator
{
    /// <summary>Format tag — "pdf", "xlsx", "csv".</summary>
    string Format { get; }

    Task<byte[]> GenerateAsync(
        ReportRequest request, CancellationToken ct = default);
}

public sealed record ReportRequest(
    string Title,
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyList<string>> Rows,
    string? Summary = null);
