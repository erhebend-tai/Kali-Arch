using CleanArch.Application.Interfaces.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CleanArch.Application.Reports.Generators;

public sealed class PdfReportGenerator : IReportGenerator
{
    public string Format => "pdf";

    public Task<byte[]> GenerateAsync(
        ReportRequest request, CancellationToken ct = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(ComposeHeader);
                page.Content().Element(content =>
                    ComposeContent(content, request));

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    private static void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("CleanArch Platform")
                    .FontSize(20).Bold()
                    .FontColor(Colors.Blue.Medium);
                col.Item().Text(
                        $"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC")
                    .FontSize(9).FontColor(Colors.Grey.Medium);
            });
        });
    }

    private static void ComposeContent(
        IContainer container, ReportRequest request)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Item().Text(request.Title).FontSize(16).Bold();

            col.Item().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    foreach (var _ in request.Columns)
                        cols.RelativeColumn();
                });

                table.Header(header =>
                {
                    foreach (var column in request.Columns)
                        header.Cell()
                            .Background(Colors.Blue.Medium)
                            .Padding(5)
                            .Text(column)
                            .FontColor(Colors.White).Bold();
                });

                foreach (var row in request.Rows)
                {
                    foreach (var cell in row)
                        table.Cell()
                            .BorderBottom(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(5)
                            .Text(cell);
                }
            });

            if (request.Summary is not null)
            {
                col.Item().PaddingTop(20)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Text(request.Summary);
            }
        });
    }
}
