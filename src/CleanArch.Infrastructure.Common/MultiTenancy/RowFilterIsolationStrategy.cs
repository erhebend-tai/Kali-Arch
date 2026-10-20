using CleanArch.Application.MultiTenancy;
using CleanArch.Application.MultiTenancy.Isolation;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Common.MultiTenancy;

/// <summary>
/// Row-level isolation — one schema, EF Core global query
/// filter on tenant_id. Cheapest; suits high-tenant-count.
/// </summary>
public sealed class RowFilterIsolationStrategy(
    ITenantContext tenant, string connectionString)
    : ITenantIsolationStrategy
{
    public void Apply()
    {
        // Registered in DbContext.OnModelCreating:
        //   entity.HasQueryFilter(e => EF.Property<string>(e,
        //       "TenantId") == _tenant.TenantId)
        // This strategy just validates the context is present.
        if (!tenant.HasTenant)
            throw new InvalidOperationException(
                "Row-filter isolation requires a tenant context.");
    }

    public string GetConnectionString() => connectionString;
}
