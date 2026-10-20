using System.Text.RegularExpressions;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.ValueObjects.Compute;

/// <summary>
/// VM / resource name — lowercase alphanumeric + hyphens, max 63 chars.
/// Self-validating value object.
/// </summary>
public sealed record ResourceName
{
    private static readonly Regex Pattern = new(
        "^[a-z0-9]([a-z0-9-]{0,61}[a-z0-9])?$",
        RegexOptions.Compiled);

    public string Value { get; }

    public ResourceName(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Pattern.IsMatch(value))
            throw new InvalidDomainStateException(
                $"'{value}' is not a valid resource name " +
                "(lowercase alphanumeric + hyphens, max 63 chars).");

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(ResourceName name) => name.Value;
}
