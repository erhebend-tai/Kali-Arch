using CleanArch.Application.Interfaces.Scraping;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace CleanArch.Infrastructure.Common.Scraping;

/// <summary>
/// Headless-browser scraper — handles JavaScript-rendered
/// pages and selector-based extraction.
/// </summary>
public sealed class PlaywrightWebScraperService(
    ILogger<PlaywrightWebScraperService> logger)
    : IWebScraperService
{
    public async Task<ScrapeResult> ScrapeAsync(
        ScrapeRequest request, CancellationToken ct = default)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium
            .LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

        var page = await browser.NewPageAsync();
        await page.GotoAsync(request.Url,
            new PageGotoOptions { Timeout = (float)request.Timeout.TotalMilliseconds });

        if (request.WaitForSelector is not null)
            await page.WaitForSelectorAsync(request.WaitForSelector);

        var title = await page.TitleAsync();
        var content = await page.ContentAsync();

        var data = new Dictionary<string, string>();
        foreach (var (name, selector) in request.Selectors)
        {
            var element = await page.QuerySelectorAsync(selector);
            data[name] = element is null
                ? string.Empty
                : await element.InnerTextAsync();
        }

        logger.LogInformation(
            "Scraped {Url} — {Count} selectors matched.",
            request.Url, data.Values.Count(v => v.Length > 0));

        return new ScrapeResult(
            request.Url, title, content, data, DateTime.UtcNow);
    }
}
