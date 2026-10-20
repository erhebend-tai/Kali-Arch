using System.Globalization;
using System.Text;
using CleanArch.Application.Interfaces.Reports;
using CsvHelper;

namespace CleanArch.Application.Reports.Generators;

public sealed class CsvReportGenerator : IReportGenerator
{
    public string Format => "csv";

    public Task<byte[]> GenerateAsync(
        ReportRequest request, CancellationToken ct = default)
    {
        using var stream = new MemoryStream();
        using (var writer = new StreamWriter(stream, Encoding.UTF8,
                   leaveOpen: true))
        using (var csv = new CsvWriter(writer,
                   CultureInfo.InvariantCulture, leaveOpen: true))
        {
            foreach (var column in request.Columns)
                csv.WriteField(column);
            csv.NextRecord();

            foreach (var row in request.Rows)
            {
                foreach (var cell in row)
                    csv.WriteField(cell);
                csv.NextRecord();
            }
        }

        return Task.FromResult(stream.ToArray());
    }
}
