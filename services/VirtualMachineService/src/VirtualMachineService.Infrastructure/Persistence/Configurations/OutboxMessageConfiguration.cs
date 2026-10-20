using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualMachineService.Infrastructure.Outbox;

namespace VirtualMachineService.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration
    : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> entity)
    {
        entity.ToTable("messages", "outbox");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.Topic)
            .HasColumnName("topic").HasMaxLength(200).IsRequired();
        entity.Property(e => e.PayloadType)
            .HasColumnName("payload_type").HasMaxLength(500).IsRequired();
        entity.Property(e => e.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb").IsRequired();
        entity.Property(e => e.TenantId)
            .HasColumnName("tenant_id").HasMaxLength(100);
        entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
        entity.Property(e => e.RetryCount).HasColumnName("retry_count");
        entity.Property(e => e.NextRetryAt).HasColumnName("next_retry_at");
        entity.Property(e => e.Error).HasColumnName("error");
        entity.Property(e => e.CorrelationId)
            .HasColumnName("correlation_id").HasMaxLength(100);
    }
}
