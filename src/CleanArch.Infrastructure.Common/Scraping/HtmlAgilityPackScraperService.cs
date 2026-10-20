using CleanArch.Application.Interfaces.Scraping;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Scraping;

/// <summary>
/// Lightweight HTTP+HTML scraper — static pages only,
/// no browser. Cheap alternative to Playwright.
/// </summary>
public sealed class HtmlAgilityPackScraperService(
    HttpClient http,
    ILogger<HtmlAgilityPackScraperService> logger)
    : IWebScraperService
{
    public async Task<ScrapeResult> ScrapeAsync(
        ScrapeRequest request, CancellationToken ct = default)
    {
        var html = await http.GetStringAsync(request.Url, ct);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var title = doc.DocumentNode
            .SelectSingleNode("//title")?.InnerText.Trim() ?? string.Empty;

        var data = new Dictionary<string, string>();
        foreach (var (name, xpath) in request.Selectors)
        {
            var node = doc.DocumentNode.SelectSingleNode(xpath);
            data[name] = node?.InnerText.Trim() ?? string.Empty;
        }

        logger.LogInformation(
            "Scraped {Url} via HtmlAgilityPack.", request.Url);

        return new ScrapeResult(
            request.Url, title, html, data, DateTime.UtcNow);
    }
}
