using VirtualMachineService.Domain.Exceptions;

namespace VirtualMachineService.Domain.ValueObjects.Compute;

/// <summary>Deployment region identifier, e.g. "eastus", "westeurope".</summary>
public sealed record Region
{
    public string Value { get; }

    public Region(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            throw new InvalidDomainStateException(
                $"'{value}' is not a valid region.");

        Value = value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Region region) => region.Value;
}
