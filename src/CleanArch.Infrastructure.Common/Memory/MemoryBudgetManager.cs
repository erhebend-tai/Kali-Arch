using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Common.Memory;

/// <summary>
/// Per-operation memory budget — refuses work that would
/// push the process over its allocation ceiling.
/// </summary>
public sealed class MemoryBudgetManager(
    ILogger<MemoryBudgetManager> logger)
{
    private readonly ConcurrentDictionary<string, long> _reservations = new();
    private readonly long _budgetBytes = 2L * 1024 * 1024 * 1024; // 2GB

    public bool TryReserve(
        string operationId,
        long estimatedBytes,
        out long remainingBudget)
    {
        var currentUsage = _reservations.Values.Sum();

        if (currentUsage + estimatedBytes > _budgetBytes)
        {
            logger.LogWarning(
                "Memory budget exceeded: {Usage}/{Budget} bytes. " +
                "Rejecting {OperationId} ({Estimated} bytes).",
                currentUsage, _budgetBytes,
                operationId, estimatedBytes);
            remainingBudget = _budgetBytes - currentUsage;
            return false;
        }

        _reservations[operationId] = estimatedBytes;
        remainingBudget = _budgetBytes - currentUsage - estimatedBytes;
        return true;
    }

    public void Release(string operationId) =>
        _reservations.TryRemove(operationId, out _);
}
