using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using VirtualMachineService.Application.Common.Models;
using VirtualMachineService.Application.DTOs.Compute;
using VirtualMachineService.Application.Interfaces;
using VirtualMachineService.Domain.Entities.Compute;
using VirtualMachineService.Domain.Exceptions;

namespace VirtualMachineService.Infrastructure.Persistence.Repositories;

public sealed class VirtualMachineRepository(
    AppDbContext context,
    IDbConnectionFactory connectionFactory,
    ILogger<VirtualMachineRepository> logger)
    : IVirtualMachineRepository
{
    // EF Core for standard CRUD
    public async Task<VirtualMachine?> GetByIdAsync(
        Guid id, CancellationToken ct = default) =>
        await context.VirtualMachines
            .FirstOrDefaultAsync(vm => vm.Id == id, ct);

    // Stored procedure via Dapper — complex atomic operation
    public async Task ProvisionAsync(
        VirtualMachine vm,
        string correlationId,
        CancellationToken ct = default)
    {
        await using var conn = await connectionFactory
            .CreateConnectionAsync(ct);

        try
        {
            await conn.ExecuteAsync(
                "CALL vm_service.sp_provision_vm(" +
                "@id, @tenantId, @name, @region, @size, " +
                "@osType, @resourceGroup, @provider, " +
                "@createdBy, @correlationId)",
                new
                {
                    id            = vm.Id,
                    tenantId      = vm.TenantId,
                    name          = vm.Name.Value,
                    region        = vm.Region.Value,
                    size          = vm.Size.Value,
                    osType        = vm.OsType.ToString(),
                    resourceGroup = vm.ResourceGroup,
                    provider      = vm.Provider,
                    createdBy     = vm.CreatedBy,
                    correlationId
                });
        }
        catch (PostgresException ex)
            when (ex.Message.StartsWith("QUOTA_EXCEEDED"))
        {
            throw new QuotaExceededException(vm.TenantId);
        }
        catch (PostgresException ex)
            when (ex.Message.StartsWith("DUPLICATE_VM"))
        {
            throw new ResourceAlreadyExistsException(vm.Name.Value);
        }
    }

    // Table-valued function via Dapper — paginated read
    public async Task<PagedResult<VmSummaryDto>> GetByTenantAsync(
        string tenantId,
        VmFilter filter,
        CancellationToken ct = default)
    {
        await using var conn = await connectionFactory
            .CreateConnectionAsync(ct);

        var rows = await conn.QueryAsync<VmSummaryRow>(
            "SELECT * FROM vm_service.sp_get_vms_by_tenant(" +
            "@tenantId, @status, @region, @search, @page, @pageSize)",
            new
            {
                tenantId = tenantId,
                status   = filter.Status,
                region   = filter.Region,
                search   = filter.Search,
                page     = filter.Page,
                pageSize = filter.PageSize
            });

        var list = rows.ToList();
        return new PagedResult<VmSummaryDto>(
            Items:      list.Select(r => r.ToSummary()),
            TotalCount: list.FirstOrDefault()?.TotalCount ?? 0,
            Page:       filter.Page,
            PageSize:   filter.PageSize);
    }

    // View query — read model
    public async Task<IEnumerable<VmsByRegionDto>> GetActiveByRegionAsync(
        CancellationToken ct = default)
    {
        await using var conn = await connectionFactory
            .CreateConnectionAsync(ct);

        return await conn.QueryAsync<VmsByRegionDto>(
            "SELECT * FROM vm_service.vw_active_vms_by_region");
    }

    public Task UpdateAsync(
        VirtualMachine vm, CancellationToken ct = default)
    {
        context.VirtualMachines.Update(vm);
        return context.SaveChangesAsync(ct);
    }

    // LISTEN/NOTIFY — real-time outbox wake-up
    public async Task ListenForOutboxNotificationsAsync(
        Func<string, Task> onNotification,
        CancellationToken ct = default)
    {
        await using var conn = (NpgsqlConnection)
            await connectionFactory.CreateConnectionAsync(ct);

        await conn.ExecuteAsync("LISTEN outbox_new_message");

        conn.Notification += async (_, args) =>
            await onNotification(args.Payload);

        while (!ct.IsCancellationRequested)
            await conn.WaitAsync(ct);
    }

    private sealed class VmSummaryRow
    {
        public Guid     Id            { get; init; }
        public string   Name          { get; init; } = string.Empty;
        public string   Region        { get; init; } = string.Empty;
        public string   Size          { get; init; } = string.Empty;
        public string   OsType        { get; init; } = string.Empty;
        public string   Status        { get; init; } = string.Empty;
        public string   ResourceGroup { get; init; } = string.Empty;
        public DateTime CreatedAt     { get; init; }
        public long     TotalCount    { get; init; }

        public VmSummaryDto ToSummary() =>
            new(Id, Name, Region, Size, OsType, Status,
                ResourceGroup, CreatedAt);
    }
}
