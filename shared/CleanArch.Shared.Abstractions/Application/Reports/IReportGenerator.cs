using CleanArch.Shared.Abstractions.DTOs.Reports;

namespace CleanArch.Shared.Abstractions.Application.Reports;

public interface IReportGenerator
{
    /// <summary>Format tag - "pdf", "xlsx", "csv".</summary>
    string Format { get; }

    Task<byte[]> GenerateAsync(
        ReportRequest request, CancellationToken ct = default);
}
