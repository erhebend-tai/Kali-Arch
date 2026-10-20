using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VirtualMachineService.Application.Webhooks;

namespace VirtualMachineService.Infrastructure.Persistence.Configurations;

public sealed class WebhookSubscriptionConfiguration
    : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> entity)
    {
        entity.ToTable("webhook_subscriptions", "vm_service");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).HasColumnName("id");
        entity.Property(e => e.TenantId)
            .HasColumnName("tenant_id").HasMaxLength(100).IsRequired();
        entity.Property(e => e.TargetUrl)
            .HasColumnName("target_url").HasMaxLength(2048).IsRequired();
        entity.Property(e => e.Secret)
            .HasColumnName("secret").HasMaxLength(256).IsRequired();
        entity.Property(e => e.EventTypes)
            .HasColumnName("event_types")
            .HasColumnType("text[]").IsRequired();
        entity.Property(e => e.IsActive).HasColumnName("is_active");
        entity.Property(e => e.MaxRetries).HasColumnName("max_retries");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        entity.Property(e => e.LastTriggeredAt)
            .HasColumnName("last_triggered_at");
        entity.Property(e => e.FailureCount).HasColumnName("failure_count");
    }
}
