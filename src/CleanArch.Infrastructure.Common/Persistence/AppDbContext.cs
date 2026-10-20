using CleanArch.Domain.Entities.Compute;
using CleanArch.Shared.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Common.Persistence;

/// <summary>
/// Provider-agnostic EF Core context — provider packages add their
/// own UseSqlServer/UseNpgsql/etc. at registration time.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<VirtualMachine> VirtualMachines =>
        Set<VirtualMachine>();
    public DbSet<Outbox.OutboxMessage> OutboxMessages =>
        Set<Outbox.OutboxMessage>();
    public DbSet<Inbox.InboxMessage> InboxMessages =>
        Set<Inbox.InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VirtualMachine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ResourceGroup).HasMaxLength(256);
            entity.Property(e => e.CreatedBy).HasMaxLength(256);
            entity.OwnsOne(e => e.Name, n =>
                n.Property(v => v.Value).HasColumnName("Name").HasMaxLength(63));
            entity.OwnsOne(e => e.Region, n =>
                n.Property(v => v.Value).HasColumnName("Region").HasMaxLength(64));
            entity.OwnsOne(e => e.Size, n =>
                n.Property(v => v.Value).HasColumnName("Size").HasMaxLength(32));
            entity.Ignore(e => e.DomainEvents);
        });

        modelBuilder.Entity<Outbox.OutboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasMaxLength(512);
            entity.Property(e => e.Error).HasMaxLength(2048);
        });

        modelBuilder.Entity<Inbox.InboxMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EventId).IsUnique();
            entity.Property(e => e.EventId).HasMaxLength(512);
            entity.Property(e => e.EventType).HasMaxLength(512);
            entity.Property(e => e.Error).HasMaxLength(2048);
        });

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>Entities with pending domain events, for UoW dispatch.</summary>
    public IReadOnlyList<AuditableEntity> EntitiesWithEvents =>
        ChangeTracker.Entries<AuditableEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();
}
