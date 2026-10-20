using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CleanArch.IntegrationTests.Api;

/// <summary>
/// WebApplicationFactory boots the real Program.cs —
/// middleware, model binding, validation, ProblemDetails.
/// </summary>
public sealed class VmApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public VmApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Provision_InvalidInput_Returns400ProblemDetails()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/compute/vms",
            new
            {
                name = "",          // invalid
                resourceGroup = "",
                region = "moon-1",
                osType = "Amiga",
                size = "huge"
            });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType
            .Should().Be("application/problem+json");

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("code");
    }

    [Fact]
    public async Task Health_Liveness_ReturnsOk()
    {
        var response = await _client.GetAsync("/healthz/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
