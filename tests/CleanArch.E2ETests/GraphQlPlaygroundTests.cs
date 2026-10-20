using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace CleanArch.E2ETests;

/// <summary>
/// Browser-level tests — requires `pwsh bin/Debug/net10.0/
/// playwright.ps1 install` once per machine, plus the API
/// running at BaseUrl.
/// </summary>
public sealed class GraphQlPlaygroundTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;

    private static string BaseUrl =>
        Environment.GetEnvironmentVariable("E2E_BASE_URL")
        ?? "http://localhost:5000";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium
            .LaunchAsync(new() { Headless = true });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task GraphQL_Endpoint_ServesSchema()
    {
        var page = await _browser.NewPageAsync();
        var response = await page.GotoAsync(
            $"{BaseUrl}/graphql?sdl");

        response!.Ok.Should().BeTrue();
        var body = await page.ContentAsync();
        body.Should().Contain("type Query");
    }

    [Fact]
    public async Task OpenApi_Document_Loads()
    {
        var page = await _browser.NewPageAsync();
        var response = await page.GotoAsync(
            $"{BaseUrl}/openapi/v1.json");

        response!.Ok.Should().BeTrue();
    }
}
