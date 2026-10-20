using CleanArch.Application.MultiTenancy;
using CleanArch.Application.MultiTenancy.Isolation;

namespace CleanArch.Infrastructure.Common.MultiTenancy;

/// <summary>
/// Database-per-tenant — strongest isolation, highest cost.
/// Connection string template carries a {tenant} placeholder.
/// </summary>
public sealed class DatabaseIsolationStrategy(
    ITenantContext tenant, string connectionStringTemplate)
    : ITenantIsolationStrategy
{
    public void Apply()
    {
        if (!tenant.HasTenant)
            throw new InvalidOperationException(
                "Database isolation requires a tenant context.");
    }

    public string GetConnectionString() =>
        connectionStringTemplate.Replace(
            "{tenant}", tenant.TenantId);
}
