namespace CleanArch.Application.MultiTenancy;

/// <summary>
/// Ambient tenant — set by TenantMiddleware (or the equivalent
/// in Functions/workers) and read by repositories and strategies.
/// </summary>
public interface ITenantContext
{
    string? TenantId { get; }
    void SetTenant(string tenantId);
    bool HasTenant { get; }
}

public sealed class TenantContext : ITenantContext
{
    private readonly AsyncLocal<string?> _tenantId = new();

    public string? TenantId => _tenantId.Value;
    public bool HasTenant => _tenantId.Value is not null;

    public void SetTenant(string tenantId) =>
        _tenantId.Value = tenantId;
}
