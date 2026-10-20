using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.ValueObjects.Compute;

/// <summary>
/// Cloud region — validated against the platform's supported set.
/// </summary>
public sealed record Region
{
    private static readonly HashSet<string> Supported =
    [
        "us-east", "us-west",
        "europe-west", "europe-north",
        "asia-east", "asia-southeast"
    ];

    public string Value { get; }

    public Region(string value)
    {
        if (!Supported.Contains(value))
            throw new InvalidDomainStateException(
                $"'{value}' is not a supported region.");

        Value = value;
    }

    public override string ToString() => Value;
}
