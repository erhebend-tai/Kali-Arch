namespace CleanArch.Shared.Abstractions.Application.MultiTenancy;

/// <summary>
/// How a tenant's data is segregated -
/// row filter, schema, or separate database.
/// </summary>
public interface ITenantIsolationStrategy
{
    TenantIsolationMode Mode { get; }

    /// <summary>Applies isolation to the persistence context.</summary>
    void Apply();

    /// <summary>Connection string for the current tenant's store.</summary>
    string GetConnectionString();
}

public enum TenantIsolationMode
{
    Row,
    Schema,
    Database
}
