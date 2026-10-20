using VirtualMachineService.Application.Common.Models;
using VirtualMachineService.Application.DTOs.Compute;
using VirtualMachineService.Domain.Entities.Compute;

namespace VirtualMachineService.Application.Interfaces;

/// <summary>
/// Read/write repository contract. EF Core handles standard CRUD;
/// stored procedures (via Dapper) handle atomic multi-write
/// operations like provision-with-outbox.
/// </summary>
public interface IVirtualMachineRepository
{
    Task<VirtualMachine?> GetByIdAsync(
        Guid id, CancellationToken ct = default);

    Task ProvisionAsync(
        VirtualMachine vm,
        string correlationId,
        CancellationToken ct = default);

    Task<PagedResult<VmSummaryDto>> GetByTenantAsync(
        string tenantId,
        VmFilter filter,
        CancellationToken ct = default);

    Task<IEnumerable<VmsByRegionDto>> GetActiveByRegionAsync(
        CancellationToken ct = default);

    Task UpdateAsync(
        VirtualMachine vm, CancellationToken ct = default);

    /// <summary>
    /// LISTEN on outbox_new_message — wakes the outbox processor
    /// the moment a row lands instead of polling.
    /// </summary>
    Task ListenForOutboxNotificationsAsync(
        Func<string, Task> onNotification,
        CancellationToken ct = default);
}
