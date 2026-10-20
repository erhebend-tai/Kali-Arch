using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualMachineService.Domain.Entities.Compute;

namespace VirtualMachineService.Infrastructure.Persistence.Configurations;

/// <summary>Maps the aggregate to vm_service.virtual_machines.</summary>
public sealed class VirtualMachineConfiguration
    : IEntityTypeConfiguration<VirtualMachine>
{
    public void Configure(EntityTypeBuilder<VirtualMachine> entity)
    {
        entity.ToTable("virtual_machines", "vm_service");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.TenantId)
            .HasColumnName("tenant_id").HasMaxLength(100).IsRequired();
        entity.Property(e => e.OsType)
            .HasColumnName("os_type").HasMaxLength(10)
            .HasConversion<string>();
        entity.Property(e => e.Status)
            .HasColumnName("status").HasMaxLength(20)
            .HasConversion(
                s => s.ToString().ToLowerInvariant(),
                s => Enum.Parse<Domain.Enums.Compute.VmStatus>(
                    s, ignoreCase: true));
        entity.Property(e => e.ResourceGroup)
            .HasColumnName("resource_group").HasMaxLength(90).IsRequired();
        entity.Property(e => e.Provider)
            .HasColumnName("provider").HasMaxLength(20).IsRequired();
        entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        entity.Property(e => e.CreatedBy)
            .HasColumnName("created_by").HasMaxLength(200).IsRequired();
        entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        entity.Property(e => e.UpdatedBy)
            .HasColumnName("updated_by").HasMaxLength(200);
        entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
        entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
        entity.Property(e => e.DeletedBy)
            .HasColumnName("deleted_by").HasMaxLength(200);
        entity.Property(e => e.RowVersion)
            .HasColumnName("row_version")
            .IsConcurrencyToken();

        entity.OwnsOne(e => e.Name, n =>
            n.Property(v => v.Value)
                .HasColumnName("name").HasMaxLength(63).IsRequired());
        entity.OwnsOne(e => e.Region, n =>
            n.Property(v => v.Value)
                .HasColumnName("region").HasMaxLength(50).IsRequired());
        entity.OwnsOne(e => e.Size, n =>
            n.Property(v => v.Value)
                .HasColumnName("size").HasMaxLength(20).IsRequired());

        entity.HasMany<VmSnapshot>()
            .WithOne()
            .HasForeignKey(s => s.VmId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.Ignore(e => e.DomainEvents);

        // Soft-delete filter — mirrors is_deleted checks in SQL.
        entity.HasQueryFilter(e => !e.IsDeleted);
    }
}
