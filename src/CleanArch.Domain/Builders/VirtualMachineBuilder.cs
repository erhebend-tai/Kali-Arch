using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Factories;

namespace CleanArch.Domain.Builders;

/// <summary>
/// Fluent builder for complex entity construction.
/// Terminal Build() delegates to the factory for validation.
/// </summary>
public sealed class VirtualMachineBuilder
{
    private string _name = string.Empty;
    private string _region = string.Empty;
    private string _size = "medium";
    private string _osType = "Linux";
    private string _resourceGroup = string.Empty;
    private string _createdBy = string.Empty;
    private readonly List<string> _tags = [];

    public VirtualMachineBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public VirtualMachineBuilder InRegion(string region)
    {
        _region = region;
        return this;
    }

    public VirtualMachineBuilder WithSize(string size)
    {
        _size = size;
        return this;
    }

    public VirtualMachineBuilder RunningLinux()
    {
        _osType = "Linux";
        return this;
    }

    public VirtualMachineBuilder RunningWindows()
    {
        _osType = "Windows";
        return this;
    }

    public VirtualMachineBuilder InResourceGroup(string resourceGroup)
    {
        _resourceGroup = resourceGroup;
        return this;
    }

    public VirtualMachineBuilder CreatedBy(string user)
    {
        _createdBy = user;
        return this;
    }

    public VirtualMachineBuilder WithTag(string tag)
    {
        _tags.Add(tag);
        return this;
    }

    public IReadOnlyList<string> Tags => _tags;

    public VirtualMachine Build(IVirtualMachineFactory factory) =>
        factory.Create(_name, _region, _size,
            _osType, _resourceGroup, _createdBy);
}
