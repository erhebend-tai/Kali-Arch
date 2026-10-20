namespace CleanArch.Application.Interfaces;

public interface IDashboardService
{
    Task IncrementVmCountAsync(CancellationToken ct = default);
    Task DecrementVmCountAsync(CancellationToken ct = default);
}
