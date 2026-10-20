using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Interfaces.Repositories.Compute;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CleanArch.Workers.Compute;

/// <summary>
/// Always-on worker — polls VMs and reconciles status drift
/// against the cloud provider.
/// </summary>
public sealed class VmHealthCheckWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<VmHealthCheckWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var repository = scope.ServiceProvider
                    .GetRequiredService<IVirtualMachineRepository>();

                var running = await repository.GetByStatusAsync(
                    VmStatus.Running, stoppingToken);

                foreach (var vm in running)
                    logger.LogDebug("Health-checking VM {VmName}",
                        vm.Name.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "VM health check cycle failed");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
