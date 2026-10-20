using CleanArch.Shared.Abstractions.DTOs.Scraping;

namespace CleanArch.Shared.Abstractions.Application.Scraping;

public interface IWebScraperService
{
    Task<ScrapeResult> ScrapeAsync(
        ScrapeRequest request, CancellationToken ct = default);
}
