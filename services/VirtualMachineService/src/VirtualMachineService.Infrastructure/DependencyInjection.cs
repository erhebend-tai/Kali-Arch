using CleanArch.Shared.Grpc.Ai;
using CleanArch.Shared.Grpc.Identity;
using CleanArch.Shared.Grpc.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtualMachineService.Application.Interfaces;
using VirtualMachineService.Application.Interfaces.Webhooks;
using VirtualMachineService.Application.Reactive.Streams;
using VirtualMachineService.Infrastructure.Clients;
using VirtualMachineService.Infrastructure.Persistence;
using VirtualMachineService.Infrastructure.Persistence.Repositories;
using VirtualMachineService.Infrastructure.Webhooks;

namespace VirtualMachineService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddVirtualMachineInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("VirtualMachineDb")
            ?? throw new InvalidOperationException(
                "Connection string 'VirtualMachineDb' is missing.");

        // EF Core — Npgsql provider, schemas owned by the SQL scripts.
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(connectionString));

        // Raw connections for Dapper / LISTEN-NOTIFY.
        services.AddSingleton<IDbConnectionFactory>(
            new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<IVirtualMachineRepository,
            VirtualMachineRepository>();

        // Webhooks — verifier is stateless; service+registry are scoped.
        services.AddSingleton<IWebhookVerifier, WebhookVerifier>();
        services.AddScoped<IWebhookRegistry, WebhookRegistry>();
        services.AddHttpClient<WebhookService>();
        services.AddTransient<IWebhookService>(sp =>
            sp.GetRequiredService<WebhookService>());
        services.AddHostedService<WebhookDeliveryWorker>();

        // Migrations — DbUp scripts + EF model, on startup.
        services.AddHostedService<MigrationHostedService>();

        // Hot status stream — singleton fan-out.
        services.AddSingleton<VmStatusStream>();

        // Typed gRPC clients — auth + retry on transient failures.
        services.AddGrpcClient<IdentityService.IdentityServiceClient>(opts =>
        {
            opts.Address = new Uri(
                configuration["Services:IdentityService:GrpcEndpoint"]!);
        })
        .AddInterceptor<ClientAuthInterceptor>()
        .AddInterceptor<ClientRetryInterceptor>();

        services.AddGrpcClient<AIService.AIServiceClient>(opts =>
        {
            opts.Address = new Uri(
                configuration["Services:AIService:GrpcEndpoint"]!);
        })
        .AddInterceptor<ClientAuthInterceptor>()
        .AddInterceptor<ClientRetryInterceptor>();

        services.AddScoped<IdentityServiceClient>();
        services.AddScoped<AIServiceClient>();

        return services;
    }
}
