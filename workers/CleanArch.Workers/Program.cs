using CleanArch.FileWatcher.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CleanArch.Workers.Compute;
using CleanArch.Workers.Messaging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<VmHealthCheckWorker>();
builder.Services.AddHostedService<EventProcessorWorker>();
builder.Services.AddFileWatchers(builder.Configuration);

var host = builder.Build();
host.Run();
