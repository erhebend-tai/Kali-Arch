using CleanArch.Application.MultiTenancy;
using CleanArch.Application.MultiTenancy.Isolation;

namespace CleanArch.Infrastructure.Common.MultiTenancy;

/// <summary>
/// Schema-per-tenant — same database, each tenant's tables
/// live under tenant_{id}.*. Implemented via
/// EntityTypeBuilder.ToTable(name, schema) at model time.
/// </summary>
public sealed class SchemaIsolationStrategy(
    ITenantContext tenant, string connectionString)
    : ITenantIsolationStrategy
{
    public void Apply()
    {
        if (!tenant.HasTenant)
            throw new InvalidOperationException(
                "Schema isolation requires a tenant context.");
    }

    public string GetConnectionString() => connectionString;

    /// <summary>Schema name for the current tenant.</summary>
    public string GetSchema() =>
        $"tenant_{tenant.TenantId?.Replace("-", "_")}";
}
