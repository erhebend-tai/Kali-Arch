using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualMachineService.Infrastructure.Outbox;

namespace VirtualMachineService.Infrastructure.Persistence.Configurations;

public sealed class InboxMessageConfiguration
    : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> entity)
    {
        entity.ToTable("inbox_messages", "outbox");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.MessageId)
            .HasColumnName("message_id").HasMaxLength(200).IsRequired();
        entity.Property(e => e.Topic)
            .HasColumnName("topic").HasMaxLength(200).IsRequired();
        entity.Property(e => e.PayloadType)
            .HasColumnName("payload_type").HasMaxLength(500).IsRequired();
        entity.Property(e => e.Payload)
            .HasColumnName("payload")
            .HasColumnType("jsonb").IsRequired();
        entity.Property(e => e.ReceivedAt).HasColumnName("received_at");
        entity.Property(e => e.ProcessedAt).HasColumnName("processed_at");
        entity.Property(e => e.RetryCount).HasColumnName("retry_count");
        entity.Property(e => e.Error).HasColumnName("error");
        entity.Property(e => e.IsDuplicate).HasColumnName("is_duplicate");

        entity.HasIndex(e => e.MessageId).IsUnique();
    }
}
