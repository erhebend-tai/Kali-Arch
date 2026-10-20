using CleanArch.Application.Interfaces.Reports;
using ClosedXML.Excel;

namespace CleanArch.Application.Reports.Generators;

public sealed class ExcelReportGenerator : IReportGenerator
{
    public string Format => "xlsx";

    public Task<byte[]> GenerateAsync(
        ReportRequest request, CancellationToken ct = default)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(request.Title);

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor =
            XLColor.FromHtml("#1E40AF");
        headerRow.Style.Font.FontColor = XLColor.White;

        for (var i = 0; i < request.Columns.Count; i++)
            worksheet.Cell(1, i + 1).Value = request.Columns[i];

        for (var r = 0; r < request.Rows.Count; r++)
        {
            var row = request.Rows[r];
            for (var c = 0; c < row.Count; c++)
                worksheet.Cell(r + 2, c + 1).Value = row[c];

            if (r % 2 == 0)
                worksheet.Row(r + 2).Style.Fill
                    .BackgroundColor = XLColor.FromHtml("#F1F5F9");
        }

        worksheet.Columns().AdjustToContents();
        worksheet.RangeUsed()?.SetAutoFilter();
        worksheet.SheetView.FreezeRows(1);

        if (request.Summary is not null)
        {
            var summarySheet = workbook.Worksheets.Add("Summary");
            summarySheet.Cell(1, 1).Value = request.Summary;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return Task.FromResult(stream.ToArray());
    }
}
