using CleanArch.API.Grpc.Services;
using CleanArch.API.GraphQL;
using CleanArch.API.Hubs;
using CleanArch.Application.Behaviors;
using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Mappings;
using CleanArch.Application.MultiTenancy;
using CleanArch.Application.Options;
using CleanArch.Application.Reactive.Streams;
using CleanArch.Infrastructure.Alibaba;
using CleanArch.Infrastructure.AWS;
using CleanArch.Infrastructure.Azure;
using CleanArch.Infrastructure.BareMetal;
using CleanArch.Infrastructure.Cloudflare;
using CleanArch.Infrastructure.Common;
using CleanArch.Infrastructure.Common.Cache;
using CleanArch.Infrastructure.Common.Observability;
using CleanArch.Infrastructure.Common.Security.Authorization;
using CleanArch.Infrastructure.Common.Security.Headers;
using CleanArch.Infrastructure.Common.Security.RateLimiting;
using CleanArch.Infrastructure.GCP;
using CleanArch.Infrastructure.Oracle;
using CleanArch.Shared.Infrastructure.Middleware;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddGrpc();

builder.Services.AddGraphQLServer()
    .AddQueryType<ComputeQuery>()
    .AddMutationType<ComputeMutation>()
    .AddSubscriptionType<ComputeSubscription>()
    .AddInMemorySubscriptions();

// MediatR — handlers, behaviors, notifications from Application.
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<ProvisionVmCommand>();

    // Behavior order matters — outermost first.
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
});

// FluentValidation — scans Application for all AbstractValidators.
builder.Services.AddValidatorsFromAssemblyContaining<ProvisionVmCommand>();

// Mapster mappings.
builder.Services.AddMappings();

// Validated options — fail fast at startup, not on first use.
builder.Services.AddOptions<ComputeOptions>()
    .BindConfiguration(ComputeOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<AIOptions>()
    .BindConfiguration(AIOptions.SectionName);
builder.Services.AddOptions<CacheOptions>()
    .BindConfiguration(CacheOptions.SectionName);
builder.Services.AddOptions<SecurityOptions>()
    .BindConfiguration(SecurityOptions.SectionName);
builder.Services.AddOptions<MessagingOptions>()
    .BindConfiguration(MessagingOptions.SectionName);

// Reactive streams — singleton, fans out to SignalR/metrics/audit.
builder.Services.AddSingleton<VmStatusStream>();
builder.Services.AddHostedService<VmStatusBroadcaster>();

// Caching — Memory | Redis | Hybrid via config.
builder.Services.AddCaching(builder.Configuration);

// Multi-tenancy.
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Security.
builder.Services.AddAuthorization(AuthorizationPolicies.Configure);
builder.Services.AddSingleton<IAuthorizationHandler,
    QuotaAuthorizationHandler>();
builder.Services.AddPlatformRateLimiting(
    builder.Configuration
        .GetSection(SecurityOptions.SectionName)
        .Get<SecurityOptions>() ?? new SecurityOptions());

// Observability — OpenTelemetry + health probes.
builder.Services.AddPlatformObservability(
    builder.Configuration["Observability:OtlpEndpoint"]);
builder.Services.AddPlatformHealthChecks(builder.Configuration);

// Provider-agnostic platform services — EF, UoW, ambient user,
// messaging fallbacks the selected provider may override.
builder.Services.AddCommonInfrastructure(builder.Configuration);

// Provider selection — one switch, cloud-agnostic everything else.
var provider = builder.Configuration.GetValue<string>("CloudProvider") ?? "Azure";
switch (provider)
{
    case "Azure":
        builder.Services.AddAzureInfrastructure(builder.Configuration);
        break;
    case "AWS":
        builder.Services.AddAWSInfrastructure(builder.Configuration);
        break;
    case "GCP":
        builder.Services.AddGCPInfrastructure(builder.Configuration);
        break;
    case "Alibaba":
        builder.Services.AddAlibabaInfrastructure(builder.Configuration);
        break;
    case "Oracle":
        builder.Services.AddOracleInfrastructure(builder.Configuration);
        break;
    case "Cloudflare":
        builder.Services.AddCloudflareInfrastructure(builder.Configuration);
        break;
    case "BareMetal":
        builder.Services.AddBareMetalInfrastructure(builder.Configuration);
        break;
    default:
        throw new InvalidOperationException(
            $"Unknown cloud provider: {provider}");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();
app.MapHub<VmStatusHub>("/hubs/vm-status");
app.MapGrpcService<ComputeGrpcService>();
app.MapGraphQL("/graphql");
app.MapPlatformHealthChecks();

app.Run();

/// <summary>Exposes Program to WebApplicationFactory in tests.</summary>
public partial class Program;
