using VirtualMachineService.Domain.Exceptions;

namespace VirtualMachineService.Domain.ValueObjects.Compute;

/// <summary>
/// Provider-agnostic VM size — mirrors the chk_vm_size
/// constraint in V002__create_tables.sql.
/// </summary>
public sealed record MachineSize
{
    private static readonly HashSet<string> Allowed =
        ["small", "medium", "large", "xlarge", "xxlarge"];

    public string Value { get; }

    public MachineSize(string value)
    {
        var normalized = value?.ToLowerInvariant() ?? string.Empty;

        if (!Allowed.Contains(normalized))
            throw new InvalidDomainStateException(
                $"'{value}' is not a valid machine size " +
                $"(one of: {string.Join(", ", Allowed)}).");

        Value = normalized;
    }

    public override string ToString() => Value;

    public static implicit operator string(MachineSize size) => size.Value;
}
