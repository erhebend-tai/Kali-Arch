using Microsoft.EntityFrameworkCore;
using VirtualMachineService.Application.Interfaces.Webhooks;
using VirtualMachineService.Application.Webhooks;
using VirtualMachineService.Infrastructure.Persistence;

namespace VirtualMachineService.Infrastructure.Webhooks;

/// <summary>EF-backed subscription store — vm_service.webhook_subscriptions.</summary>
public sealed class WebhookRegistry(AppDbContext context)
    : IWebhookRegistry
{
    public async Task<WebhookSubscription> RegisterAsync(
        RegisterWebhookRequest request,
        CancellationToken ct = default)
    {
        var subscription = new WebhookSubscription
        {
            TenantId   = request.TenantId,
            TargetUrl  = request.TargetUrl,
            Secret     = request.Secret,
            EventTypes = request.EventTypes,
            MaxRetries = request.MaxRetries
        };

        context.WebhookSubscriptions.Add(subscription);
        await context.SaveChangesAsync(ct);
        return subscription;
    }

    public async Task UnregisterAsync(
        Guid subscriptionId, CancellationToken ct = default)
    {
        var subscription = await GetByIdAsync(subscriptionId, ct);
        if (subscription is null) return;

        context.WebhookSubscriptions.Remove(subscription);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsAsync(
        string eventType,
        string? tenantId = null,
        CancellationToken ct = default) =>
        await context.WebhookSubscriptions
            .Where(s => s.EventTypes.Contains(eventType))
            .Where(s => tenantId == null || s.TenantId == tenantId)
            .ToListAsync(ct);

    public async Task<WebhookSubscription?> GetByIdAsync(
        Guid subscriptionId, CancellationToken ct = default) =>
        await context.WebhookSubscriptions
            .FirstOrDefaultAsync(s => s.Id == subscriptionId, ct);

    public Task UpdateAsync(
        WebhookSubscription subscription,
        CancellationToken ct = default)
    {
        context.WebhookSubscriptions.Update(subscription);
        return context.SaveChangesAsync(ct);
    }
}
