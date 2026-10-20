namespace CleanArch.Application.MultiTenancy.Isolation;

/// <summary>
/// How a tenant's data is segregated —
/// row filter, schema, or separate database.
/// </summary>
public interface ITenantIsolationStrategy
{
    /// <summary>Applies isolation to the persistence context.</summary>
    void Apply();

    /// <summary>Connection string for the current tenant's store.</summary>
    string GetConnectionString();
}
