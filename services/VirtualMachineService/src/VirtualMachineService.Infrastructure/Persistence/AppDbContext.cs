using Microsoft.EntityFrameworkCore;
using VirtualMachineService.Application.Webhooks;
using VirtualMachineService.Domain.Entities.Compute;
using VirtualMachineService.Infrastructure.Outbox;

namespace VirtualMachineService.Infrastructure.Persistence;

/// <summary>
/// EF Core context — Npgsql provider configured at registration.
/// Raw-SQL scripts under Scripts/ remain the authoritative schema;
/// EF maps the same tables for unit-of-work operations.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<VirtualMachine> VirtualMachines =>
        Set<VirtualMachine>();
    public DbSet<VmSnapshot> VmSnapshots =>
        Set<VmSnapshot>();
    public DbSet<WebhookSubscription> WebhookSubscriptions =>
        Set<WebhookSubscription>();
    public DbSet<OutboxMessage> OutboxMessages =>
        Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages =>
        Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
