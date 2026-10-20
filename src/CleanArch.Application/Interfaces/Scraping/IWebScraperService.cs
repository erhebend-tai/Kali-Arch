namespace CleanArch.Application.Interfaces.Scraping;

public interface IWebScraperService
{
    Task<ScrapeResult> ScrapeAsync(
        ScrapeRequest request, CancellationToken ct = default);
}

public sealed record ScrapeRequest
{
    public required string Url { get; init; }
    public IReadOnlyDictionary<string, string> Selectors { get; init; }
        = new Dictionary<string, string>();
    public string? WaitForSelector { get; init; }
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
}

public sealed record ScrapeResult(
    string Url,
    string Title,
    string Content,
    IReadOnlyDictionary<string, string> Data,
    DateTime ScrapedAt);
