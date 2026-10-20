namespace CleanArch.Application.FeatureFlags;

public interface IFeatureFlagService
{
    Task<bool> IsEnabledAsync(
        string flagName, CancellationToken ct = default);

    Task<bool> IsEnabledForTenantAsync(
        string flagName, string tenantId,
        CancellationToken ct = default);

    Task<bool> IsEnabledForUserAsync(
        string flagName, string userId,
        CancellationToken ct = default);

    Task<T> GetVariantAsync<T>(
        string flagName, T defaultValue,
        CancellationToken ct = default);
}

/// <summary>Well-known flag names — no magic strings.</summary>
public static class FeatureFlags
{
    public const string NewVmProvisioningFlow = "new-vm-provisioning-flow";
    public const string OnDeviceLlm = "on-device-llm";
    public const string MultiRegionDeploy = "multi-region-deploy";
    public const string AdvancedAnalytics = "advanced-analytics";
    public const string BetaUi = "beta-ui";
}
