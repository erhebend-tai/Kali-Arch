using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualMachineService.Domain.Entities.Compute;

namespace VirtualMachineService.Infrastructure.Persistence.Configurations;

public sealed class VmSnapshotConfiguration
    : IEntityTypeConfiguration<VmSnapshot>
{
    public void Configure(EntityTypeBuilder<VmSnapshot> entity)
    {
        entity.ToTable("vm_snapshots", "vm_service");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.VmId).HasColumnName("vm_id");
        entity.Property(e => e.TenantId)
            .HasColumnName("tenant_id").HasMaxLength(100).IsRequired();
        entity.Property(e => e.Name)
            .HasColumnName("name").HasMaxLength(200).IsRequired();
        entity.Property(e => e.SizeGb).HasColumnName("size_gb");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        entity.Property(e => e.CreatedBy)
            .HasColumnName("created_by").HasMaxLength(200).IsRequired();
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");

        entity.Ignore(e => e.DomainEvents);
    }
}
