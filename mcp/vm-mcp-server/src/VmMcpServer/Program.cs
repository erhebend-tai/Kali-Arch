using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Queries.Compute;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// MCP server over stdio — agent tools call into the same
// application pipeline as HTTP/gRPC/GraphQL entry points.

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold =
    LogLevel.Trace);

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// The application layer — MediatR handlers used by every
// entry point (API, Functions, workers, and now MCP).
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblyContaining<ProvisionVmCommand>());

await builder.Build().RunAsync();
