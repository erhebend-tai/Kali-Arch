using CleanArch.Application.Factories.Compute;
using CleanArch.Application.Interfaces;
using CleanArch.Application.Pipelines.Compute;
using CleanArch.Application.Pipelines.Compute.Steps;
using CleanArch.Domain.Commands;
using CleanArch.Domain.Factories;
using CleanArch.Domain.Factories.Compute;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.Interfaces.Repositories.Compute;
using CleanArch.Infrastructure.Common.Inbox;
using CleanArch.Infrastructure.Common.Persistence;
using CleanArch.Infrastructure.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CleanArch.Infrastructure.Common;

/// <summary>
/// Provider-agnostic platform services — persistence, unit of work,
/// ambient-user plumbing, and dev/test fallbacks for messaging.
/// Cloud provider packages override the fallbacks via their own
/// registration extensions (TryAdd semantics).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCommonInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            var connectionString =
                configuration.GetConnectionString("Default");
            if (!string.IsNullOrEmpty(connectionString))
                options.UseSqlServer(connectionString);
            else
                options.UseInMemoryDatabase("cleanarch");
        });

        services.AddScoped<IVirtualMachineRepository,
            VirtualMachineRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInboxRepository, InboxRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService,
            HttpContextCurrentUserService>();

        services.AddScoped<VmProvisioningContextFactory>();

        // Provisioning pipeline — steps are resolved from DI so a
        // provider can swap individual steps without touching the chain.
        services.AddScoped<ValidateVmNameStep>();
        services.AddScoped<CheckQuotaStep>();
        services.AddScoped<ReserveResourcesStep>();
        services.AddScoped<ProvisionVmStep>();
        services.AddScoped<ConfigureNetworkStep>();
        services.AddScoped<InstallAgentStep>();
        services.AddScoped<NotifyCompletionStep>();
        services.AddScoped<VmProvisioningPipeline>();
        services.AddScoped<CommandInvoker>();

        services.AddSingleton<IVirtualMachineFactory,
            VirtualMachineFactory>();

        services.TryAddScoped<IQuotaService, InMemoryQuotaService>();
        services.TryAddScoped<IVirtualMachineService,
            LoggingVirtualMachineService>();
        services.TryAddScoped<IEmailService, LoggingEmailService>();
        services.TryAddSingleton<IDashboardService,
            InMemoryDashboardService>();
        services.TryAddSingleton<IMessageBusService,
            LoggingMessageBusService>();

        return services;
    }
}
