using CleanArch.Shared.Domain.Primitives;

namespace CleanArch.Domain.Templates;

/// <summary>
/// Template Method — defines the provisioning algorithm skeleton.
/// Subclasses fill in resource-specific steps.
/// </summary>
public abstract class ResourceProvisioningTemplate<TResource>
    where TResource : AuditableEntity
{
    public async Task<TResource> ProvisionAsync(
        string name, string region, CancellationToken ct = default)
    {
        await ValidateAsync(name, region, ct);
        await CheckAvailabilityAsync(region, ct);
        var resource = await CreateResourceAsync(name, region, ct);
        await PostProvisionAsync(resource, ct);
        return resource;
    }

    protected virtual Task ValidateAsync(
        string name, string region, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual Task CheckAvailabilityAsync(
        string region, CancellationToken ct) =>
        Task.CompletedTask;

    protected abstract Task<TResource> CreateResourceAsync(
        string name, string region, CancellationToken ct);

    protected virtual Task PostProvisionAsync(
        TResource resource, CancellationToken ct) =>
        Task.CompletedTask;
}
