using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Docs MCP — serves architecture docs, prompt templates,
// and the event contract catalog as MCP resources.

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold =
    LogLevel.Trace);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithResourcesFromAssembly()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
