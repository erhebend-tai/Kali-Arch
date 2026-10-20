namespace CleanArch.Infrastructure.Common.Options;

/// <summary>
/// Provider-neutral base — each Infrastructure.{Provider}
/// project has its own subclass with provider-specific fields.
/// </summary>
public abstract class ProviderOptions
{
    public abstract string ProviderName { get; }
    public string DefaultRegion { get; init; } = string.Empty;
    public string? CredentialProfile { get; init; }
}

public sealed class AzureOptions : ProviderOptions
{
    public const string SectionName = "Azure";
    public override string ProviderName => "Azure";
    public string? SubscriptionId { get; init; }
    public string? TenantId { get; init; }
    public string? KeyVaultUri { get; init; }
}

public sealed class AwsOptions : ProviderOptions
{
    public const string SectionName = "AWS";
    public override string ProviderName => "AWS";
    public string? AccountId { get; init; }
    public string? SecretsManagerPrefix { get; init; }
}

public sealed class GcpOptions : ProviderOptions
{
    public const string SectionName = "GCP";
    public override string ProviderName => "GCP";
    public string? ProjectId { get; init; }
    public string? ServiceAccountKeyPath { get; init; }
}

public sealed class OracleOptions : ProviderOptions
{
    public const string SectionName = "Oracle";
    public override string ProviderName => "Oracle";
    public string? CompartmentId { get; init; }
    public string? TenancyOcid { get; init; }
}

public sealed class CloudflareOptions : ProviderOptions
{
    public const string SectionName = "Cloudflare";
    public override string ProviderName => "Cloudflare";
    public string? AccountId { get; init; }
    public string? TunnelId { get; init; }
    public string? ApiToken { get; init; }
}

public sealed class AlibabaOptions : ProviderOptions
{
    public const string SectionName = "Alibaba";
    public override string ProviderName => "Alibaba";
    public string? AccessKeyId { get; init; }
}

public sealed class BareMetalOptions : ProviderOptions
{
    public const string SectionName = "BareMetal";
    public override string ProviderName => "BareMetal";
    public IReadOnlyList<string> Hosts { get; init; } = [];
    public string? SshUser { get; init; }
}
