using CleanArch.Domain.Exceptions;

namespace CleanArch.Domain.ValueObjects.Compute;

/// <summary>
/// Machine size tier — small → xxlarge.
/// </summary>
public sealed record MachineSize
{
    private static readonly string[] Ordered =
        ["small", "medium", "large", "xlarge", "xxlarge"];

    public string Value { get; }

    public MachineSize(string value)
    {
        if (!Ordered.Contains(value))
            throw new InvalidDomainStateException(
                $"'{value}' is not a valid machine size " +
                $"({string.Join(", ", Ordered)}).");

        Value = value;
    }

    public int Ordinal => Array.IndexOf(Ordered, Value);

    public bool IsLargerThan(MachineSize other) => Ordinal > other.Ordinal;

    public override string ToString() => Value;
}
