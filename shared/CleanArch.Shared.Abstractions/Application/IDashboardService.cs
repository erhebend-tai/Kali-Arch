namespace CleanArch.Shared.Abstractions.Application;

public interface IDashboardService
{
    Task IncrementVmCountAsync(CancellationToken ct = default);
    Task DecrementVmCountAsync(CancellationToken ct = default);
}
