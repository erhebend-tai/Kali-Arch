using NBomber.CSharp;
using System.Net.Http.Json;

// NBomber load test — dotnet run -c Release
// BASE_URL env var or http://localhost:5000

var baseUrl = Environment.GetEnvironmentVariable("BASE_URL")
    ?? "http://localhost:5000";

using var httpClient = new HttpClient();

var listVms = Scenario.Create("list_vms", async _ =>
{
    using var response =
        await httpClient.GetAsync($"{baseUrl}/api/compute/vms");

    return response.IsSuccessStatusCode
        ? Response.Ok(statusCode: response.StatusCode.ToString())
        : Response.Fail(statusCode: response.StatusCode.ToString());
})
.WithLoadSimulations(
    Simulation.RampingInject(rate: 10,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromSeconds(30)),
    Simulation.Inject(rate: 50,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromMinutes(1)));

var provisionVm = Scenario.Create("provision_vm", async _ =>
{
    var payload = new
    {
        name = $"load-vm-{Guid.NewGuid():N}"[..20],
        resourceGroup = "load-test",
        region = "us-east",
        osType = "Linux",
        size = "small"
    };

    using var response = await httpClient.PostAsJsonAsync(
        $"{baseUrl}/api/compute/vms", payload);

    return response.IsSuccessStatusCode
        || (int)response.StatusCode == 400 // validation rejects are still "handled"
        ? Response.Ok(statusCode: response.StatusCode.ToString())
        : Response.Fail(statusCode: response.StatusCode.ToString());
})
.WithWeight(10) // 10% of traffic
.WithLoadSimulations(
    Simulation.Inject(rate: 5,
        interval: TimeSpan.FromSeconds(1),
        during: TimeSpan.FromMinutes(1)));

NBomberRunner
    .RegisterScenarios(listVms, provisionVm)
    .Run();
