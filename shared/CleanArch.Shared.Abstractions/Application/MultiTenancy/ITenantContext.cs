namespace CleanArch.Shared.Abstractions.Application.MultiTenancy;

/// <summary>
/// Ambient tenant - set by TenantMiddleware (or the equivalent
/// in Functions/workers) and read by repositories and strategies.
/// </summary>
public interface ITenantContext
{
    string? TenantId { get; }
    string? TenantName { get; }
    bool HasTenant { get; }
    void SetTenant(string tenantId, string? tenantName = null);
}

public sealed class TenantContext : ITenantContext
{
    private readonly AsyncLocal<(string Id, string? Name)?> _tenant = new();

    public string? TenantId => _tenant.Value?.Id;
    public string? TenantName => _tenant.Value?.Name;
    public bool HasTenant => _tenant.Value is not null;

    public void SetTenant(string tenantId, string? tenantName = null) =>
        _tenant.Value = (tenantId, tenantName);
}
